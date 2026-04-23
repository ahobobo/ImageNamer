using Application;
using Application.Ports.Driven;
using Application.Ports.Driving;
using Infrastructure.ForReadingImages;
using ImageNamer.Cli.Factories;

namespace ImageNamer.Cli;

public static class ImageRenameCli
{
    public static async Task RunAsync(string[] args)
    {
        if (args.Length == 0)
        {
            PrintUsage();
            Environment.ExitCode = 1;
            return;
        }

        if (IsHelpRequested(args[0]))
        {
            PrintUsage();
            Environment.ExitCode = 0;
            return;
        }

        IForTalkingWithModel model = OllamaAgentFactory.Create();
        IForRenamingImage renamer = new ImageRenamer(new FileOpperator(), model);
        var runner = new ImageRenameRunner(renamer);

        try
        {
            ImageRenameRunResult result = await runner.RunAsync(args[0]);
            if (result.FailureCount > 0)
            {
                Environment.ExitCode = 1;
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            Environment.ExitCode = 1;
        }
    }

    private static bool IsHelpRequested(string arg)
    {
        return string.Equals(arg, "--help", StringComparison.OrdinalIgnoreCase)
            || string.Equals(arg, "-h", StringComparison.OrdinalIgnoreCase)
            || string.Equals(arg, "/?", StringComparison.OrdinalIgnoreCase);
    }

    private static void PrintUsage()
    {
        Console.WriteLine(GetUsageText());
    }

    public static string GetUsageText()
    {
        return """
            Usage:
              <executable> <file_path>
              <executable> <directory_path>
              <executable> --help
              <executable> -h
              <executable> /?

            Renames a single image file or every supported image found recursively in a directory.
            """;
    }
}
