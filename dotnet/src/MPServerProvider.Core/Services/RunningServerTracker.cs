using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Services;

/// <summary>
/// Backs the "Restart servers that were running on last shutdown" setting
/// (<see cref="AppSettings.RestoreRunningServers"/>, audit #31). Keeps
/// <see cref="AppSettings.LastRunningServers"/> current as servers start (Running) and are
/// stopped (Stopped) or deleted — so the list is right even if the app crashes — and is
/// <see cref="Freeze">frozen</see> during shutdown so the app stopping everything doesn't erase
/// it. A crashed server stays listed: it was meant to be running.
/// <see cref="RestoreAsync"/> starts the remembered servers on the next launch.
/// </summary>
public sealed class RunningServerTracker
{
    private readonly SettingsService _settings;
    private readonly object _lock = new();
    private readonly HashSet<string> _names;
    private volatile bool _frozen;

    public RunningServerTracker(SettingsService settings, ServerProcessService processes, ServerRepository? repository = null)
    {
        _settings = settings;
        _names = new HashSet<string>(settings.Settings.LastRunningServers ?? new List<string>(), StringComparer.OrdinalIgnoreCase);

        processes.StateChanged += (server, state) =>
        {
            try
            {
                if (state == ServerState.Running)
                {
                    Change(server.ServerName, add: true);
                }
                else if (state == ServerState.Stopped)
                {
                    Change(server.ServerName, add: false);
                }
            }
            catch
            {
                // Remembering is best-effort and must never break a state change.
            }
        };

        if (repository is not null)
        {
            repository.Deleted += server =>
            {
                try
                {
                    if (server is not null)
                    {
                        Change(server.ServerName, add: false);
                    }
                }
                catch
                {
                    // Best-effort.
                }
            };
        }
    }

    /// <summary>Servers remembered as running at the last shutdown.</summary>
    public IReadOnlyList<string> Remembered
    {
        get { lock (_lock) { return _names.ToList(); } }
    }

    /// <summary>Stop recording changes (call before stopping everything at app exit).</summary>
    public void Freeze() => _frozen = true;

    /// <summary>
    /// Starts every remembered server that still exists and isn't already running. Returns one
    /// entry per attempted server with <c>null</c> on success or the failure message. Never throws
    /// for an individual server.
    /// </summary>
    public async Task<IReadOnlyList<(string Name, string? Error)>> RestoreAsync(
        Func<string, ServerInstance?> findServer,
        ServerProcessService processes,
        CancellationToken ct = default)
    {
        var results = new List<(string, string?)>();
        foreach (var name in Remembered)
        {
            ct.ThrowIfCancellationRequested();
            var server = findServer(name);
            if (server is null)
            {
                Change(name, add: false);
                continue;
            }

            if (processes.StateOf(server) is ServerState.Running or ServerState.Starting)
            {
                continue;
            }

            try
            {
                await processes.StartAsync(server, ct).ConfigureAwait(false);
                results.Add((server.ServerName, null));
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                results.Add((server.ServerName, ex.Message));
            }
        }

        return results;
    }

    private void Change(string name, bool add)
    {
        if (_frozen)
        {
            return;
        }

        // Persist under the lock so concurrent changes can't be written out of order.
        lock (_lock)
        {
            var changed = add ? _names.Add(name) : _names.Remove(name);
            if (!changed)
            {
                return;
            }

            var snapshot = _names.OrderBy(n => n, StringComparer.OrdinalIgnoreCase).ToList();
            _settings.Update(s => s.LastRunningServers = snapshot);
        }
    }
}
