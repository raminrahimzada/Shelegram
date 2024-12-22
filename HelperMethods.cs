using System.Diagnostics;
using System.Text;

namespace Shelegram;

public static class HelperMethods
{
    public static async Task<bool> RunCommandWithBash(string command, Func<string, ValueTask> action, CancellationToken cancellationToken)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "/bin/bash",
            Arguments = $"-c \"{command.Replace("\"", "\\\"")}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi);
        if(process is null)
        {
            await action("Unable to start the process");
            return false;
        }
        process.OutputDataReceived += async (sender, e) =>
        {
            if(e.Data is not null)
            {
                await action(e.Data);
            }
        };
        process.ErrorDataReceived += async (sender, e) =>
        {
            if(e.Data is not null)
            {
                await action(e.Data);
            }
        };
        await process.WaitForExitAsync(cancellationToken);
        if(process.ExitCode != 0)
        {
            await action($"process existed, ExitCode={process.ExitCode}");
        }
        var buffer = new byte[1024];
        var stream = process.StandardOutput.BaseStream;
        int size;
        while((size = await stream.ReadAsync(buffer, cancellationToken)) > 0)
        {
            var content = Encoding.UTF8.GetString(buffer, 0, size);
            await action(content);
        }
        stream = process.StandardError.BaseStream;
        while((size = await stream.ReadAsync(buffer, cancellationToken)) > 0)
        {
            var content = Encoding.UTF8.GetString(buffer, 0, size);
            await action(content);
        }
        return process.ExitCode == 0;
    }
}
