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

        string filePath = args[0];

        if (!File.Exists(filePath))
        {
            Console.Error.WriteLine($"Error: File not found at {filePath}");
            PrintUsage();
            Environment.ExitCode = 1;
            return;
        }

        IForRenamingImage renamer = new ImageRenamer(new FileOpperator(), new OllamaAgent());

        try
        {
            await renamer.RenameImageAsync(filePath);
            Console.WriteLine("Image renamed successfully.");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            Environment.ExitCode = 1;
        }
    }

    private static void PrintUsage()
    {
        Console.WriteLine("Usage:");
        Console.WriteLine("  <executable> <file_path>");
    }
}
