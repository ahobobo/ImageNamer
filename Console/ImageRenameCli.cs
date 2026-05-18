using Application;
using Application.Models;
using Application.Ports.Driven;
using Application.Ports.Driving;
using ImageNamer.Cli.Factories;
using Infrastructure.ForReadingImages;

namespace ImageNamer.Cli;

public static class ImageRenameCli
{
    public static async Task RunAsync(string[] args)
    {
        await RunAsync(args, null);
    }

    public static async Task RunAsync(string[] args, ImageRenameCliDependencies? dependencies)
    {
        dependencies ??= ImageRenameCliDependencies.Default;

        ImageRenameOptionParseResult parsed = dependencies.OptionsParser.Parse(args);
        if (parsed.IsHelpRequested)
        {
            PrintUsage();
            Environment.ExitCode = 0;
            return;
        }

        if (parsed.ErrorMessage is not null)
        {
            Console.Error.WriteLine($"Error: {parsed.ErrorMessage}");
            PrintUsage();
            Environment.ExitCode = 1;
            return;
        }

        ImageRenameOptions options = parsed.Options!;

        try
        {
            ProjectLocalConfig? config = dependencies.ConfigurationLoader.Load(options.ConfigPath);
            ImageNamingPreferences preferences = dependencies.PreferenceResolver.Resolve(config, options.Overrides);
            IForTalkingWithModel model = dependencies.ModelFactory(preferences);
            IForRenamingImage renamer = dependencies.RenamerFactory(preferences, model);
            var runner = new ImageRenameRunner(renamer);

            ImageRenameRunResult result = await runner.RunAsync(options.InputPath);
            Environment.ExitCode = result.FailureCount > 0 ? 1 : 0;
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
              <executable> <file_path> [options]
              <executable> <directory_path> [options]
              <executable> --help
              <executable> -h
              <executable> /?

            Options:
              --model <name>          Override the Ollama model for this run.
              --naming <value>        Override naming convention: normal, snake, capitalized, pascal, kebab.
              --max-length <number>   Override the maximum filename stem length.
              --config <path>         Use a project-local config file instead of imagenamer.json.

            Defaults:
              Config path: imagenamer.json in the current working directory
              Model: gemma4:e2b
              Naming: normal
              Max length: 20

            Renames a single image file or every supported image found recursively in a directory.
            """;
    }
}

public sealed class ImageRenameCliDependencies
{
    public ImageRenameOptionsParser OptionsParser { get; init; } = new();

    public ImageRenameConfigurationLoader ConfigurationLoader { get; init; } = new();

    public IForResolvingImageNamingPreferences PreferenceResolver { get; init; } =
        new ImageNamingPreferenceResolver();

    public Func<ImageNamingPreferences, IForTalkingWithModel> ModelFactory { get; init; } =
        OllamaAgentFactory.Create;

    public Func<ImageNamingPreferences, IForTalkingWithModel, IForRenamingImage> RenamerFactory { get; init; } =
        static (preferences, model) => new ImageRenamer(
            new FileOperator(),
            model,
            new ImageNameFormatter(),
            preferences);

    public static ImageRenameCliDependencies Default { get; } = new();
}
