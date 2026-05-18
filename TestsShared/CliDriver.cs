using System.Diagnostics;

namespace TestsShared;

public static class CliDriver
{
    public static async Task<(int ExitCode, string Output, string Error)> RunAsync(string[] args)
    {
        // Resolve project root by looking for ImageNamer.slnx
        string current = AppContext.BaseDirectory;
        while (!File.Exists(Path.Combine(current, "ImageNamer.slnx")))
        {
            string? parent = Path.GetDirectoryName(current);
            if (parent == null || parent == current) break;
            current = parent;
        }

        string projectPath = Path.Combine(current, "Console", "Console.csproj");
        string projectDir = Path.Combine(current, "Console");

        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"run --project \"{projectPath}\" -- {string.Join(" ", args)}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = projectDir
        };

        using var process = new Process { StartInfo = startInfo };
        
        var output = new StringWriter();
        var error = new StringWriter();

        process.OutputDataReceived += (sender, e) => { if (e.Data != null) output.WriteLine(e.Data); };
        process.ErrorDataReceived += (sender, e) => { if (e.Data != null) error.WriteLine(e.Data); };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync();

        string outStr = output.ToString();
        string errStr = error.ToString();

        if (process.ExitCode != 0)
        {
            Console.WriteLine($"CLI execution failed with exit code {process.ExitCode}");
            Console.WriteLine($"Output: {outStr}");
            Console.WriteLine($"Error: {errStr}");
        }

        return (process.ExitCode, outStr, errStr);
    }
}
