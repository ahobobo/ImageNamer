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
              --naming <value>        Override naming convention: {0}.
              --max-length <number>   Override the maximum filename stem length.
              --config <path>         Use a project-local config file instead of the default config.

            Defaults:
              Config path: imagenamer.json in the current working directory, then bundled project default
              Model: gemma-4-E4B-it-qat-GGUF:UD-Q4_K_XL
              Naming: normal
              Max length: 20

            Renames a single image file or every supported image found recursively in a directory.
            """.Replace("{0}", ImageRenameOptionsParser.SupportedNamingConventionUsageText);
    }
}

public sealed class ImageRenameCliDependencies
{
    public ImageRenameOptionsParser OptionsParser { get; init; } = new();

    public ImageRenameConfigurationLoader ConfigurationLoader { get; init; } = new();

    public IForResolvingImageNamingPreferences PreferenceResolver { get; init; } =
        new ImageNamingPreferenceResolver();

    public Func<ImageNamingPreferences, IForTalkingWithModel> ModelFactory { get; init; } =
        CreateDefaultModel;

    public Func<ImageNamingPreferences, IForTalkingWithModel, IForRenamingImage> RenamerFactory { get; init; } =
        CreateDefaultRenamer;

    public static ImageRenameCliDependencies Default { get; } = new();

    private static IForTalkingWithModel CreateDefaultModel(ImageNamingPreferences preferences)
    {
        return OllamaAgentFactory.Create(preferences);
    }

    private static IForRenamingImage CreateDefaultRenamer(
        ImageNamingPreferences preferences,
        IForTalkingWithModel model)
    {
        return new ImageRenamer(
            new FileOperator(),
            model,
            new ImageNameFormatter(),
            preferences);
    }
}
