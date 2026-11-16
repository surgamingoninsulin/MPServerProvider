using System.Diagnostics;
using System.Text;

namespace MPServerProvider.Core.Providers;

internal sealed record ExternalProcessResult(int ExitCode, string Output);

internal static class ExternalProcess
{
    public static async Task<ExternalProcessResult> RunAsync(
        string fileName,
        IEnumerable<string> arguments,
        string? workingDir,
        Action<string>? onLine,
        TimeSpan timeout,
        CancellationToken ct)
    {
        var psi = new ProcessStartInfo(fileName)
        {
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        foreach (var arg in arguments)
        {
            psi.ArgumentList.Add(arg);
        }

        if (!string.IsNullOrEmpty(workingDir))
        {
            psi.WorkingDirectory = workingDir;
        }

        var output = new StringBuilder();
        var outputLock = new object();
        void OnData(string? line)
        {
            if (line is null)
            {
                return;
            }

            lock (outputLock)
            {
                output.AppendLine(line);
            }

            try
            {
                onLine?.Invoke(line);
            }
            catch
            {
                // A progress callback failure must never break the run.
            }
        }

        using var process = new Process { StartInfo = psi };
        process.OutputDataReceived += (_, e) => OnData(e.Data);
        process.ErrorDataReceived += (_, e) => OnData(e.Data);

        if (!process.Start())
        {
            throw new InvalidOperationException($"Could not start {Path.GetFileName(fileName)}.");
        }

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        try
        {
            process.StandardInput.Close();
        }
        catch
        {
            // Already exited.
        }

        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeoutCts.CancelAfter(timeout);
        try
        {
            await process.WaitForExitAsync(timeoutCts.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            Kill(process);
            if (ct.IsCancellationRequested)
            {
                throw new OperationCanceledException(ct);
            }

            throw new TimeoutException(
                $"{Path.GetFileName(fileName)} did not finish within {timeout.TotalMinutes:0} minutes and was stopped.");
        }

        // Let the async readers drain the final lines.
        process.WaitForExit();

        lock (outputLock)
        {
            return new ExternalProcessResult(process.ExitCode, output.ToString());
        }
    }

    private static void Kill(Process process)
    {
        try
        {
            if (!process.HasExited)
            {
                process.Kill(entireProcessTree: true);
                process.WaitForExit(5000);
            }
        }
        catch
        {
            // Already gone.
        }
    }
}
