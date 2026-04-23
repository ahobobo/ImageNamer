using Application;
using Application.Ports.Driving;
using Infrastructure.ForReadingImages;
using Infrastructure.ForTalkingWithModels;

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

        IForRenamingImage renamer = new ImageRenamer(new FileOpperator(), new OllamaAgent());
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

            Renames a single image file or every supported image found recursively in a directory.
            """;
    }
}
