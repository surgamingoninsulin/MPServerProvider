using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Services;

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

    public IReadOnlyList<string> Remembered
    {
        get { lock (_lock) { return _names.ToList(); } }
    }

    public void Freeze() => _frozen = true;

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
