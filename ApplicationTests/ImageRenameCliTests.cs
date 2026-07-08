using Application;
using Application.Models;
using Application.Ports.Driven;
using Application.Ports.Driving;
using ImageNamer.Cli;
using TestsShared;

namespace ApplicationTests;

[NonParallelizable]
public class ImageRenameCliTests
{
    [Test]
    public async Task RunAsync_WithHelpFlag_PrintsUsageAndExitsSuccessfully()
    {
        TextWriter originalOut = Console.Out;
        int originalExitCode = Environment.ExitCode;
        var output = new StringWriter();

        try
        {
            Console.SetOut(output);

            await ImageRenameCli.RunAsync(["--help"]);

            Assert.That(Environment.ExitCode, Is.EqualTo(0));
            Assert.That(output.ToString(), Does.Contain("Usage:"));
            Assert.That(output.ToString(), Does.Contain("<directory_path>"));
            Assert.That(output.ToString(), Does.Contain("--help"));
            Assert.That(output.ToString(), Does.Contain("--model"));
        }
        finally
        {
            Environment.ExitCode = originalExitCode;
            Console.SetOut(originalOut);
        }
    }

    [Test]
    public async Task RunAsync_WithMissingConfig_UsesBundledDefaultConfig()
    {
        using var temp = new TemporaryWorkingDirectory();
        string imagePath = Path.Combine(temp.Path, "image.webp");
        File.WriteAllBytes(imagePath, [1, 2, 3, 4]);
        var state = new RecordingState();
        ImageRenameCliDependencies dependencies = CreateRecordingDependencies(state);
        ImageNamingPreferences expectedPreferences = LoadExpectedPreferences();

        await ImageRenameCli.RunAsync([imagePath], dependencies);

        Assert.That(state.ReceivedPreferences, Is.EqualTo(expectedPreferences));
        Assert.That(Environment.ExitCode, Is.EqualTo(0));
    }

    [Test]
    public async Task RunAsync_WithInvalidProjectConfig_ExitsBeforeRename()
    {
        using var temp = new TemporaryWorkingDirectory();
        temp.WriteConfig("""{ "maxLength": 2 }""");
        var state = new RecordingState();
        ImageRenameCliDependencies dependencies = CreateRecordingDependencies(state);
        TextWriter originalError = Console.Error;
        var error = new StringWriter();

        try
        {
            Console.SetError(error);

            await ImageRenameCli.RunAsync(["image.webp"], dependencies);

            Assert.That(Environment.ExitCode, Is.EqualTo(1));
            Assert.That(error.ToString(), Does.Contain("Maximum name length"));
            Assert.That(state.RunnerWasCreated, Is.False);
        }
        finally
        {
            Console.SetError(originalError);
        }
    }

    [Test]
    public async Task RunAsync_WithInvalidOverride_ExitsBeforeRunnerConstruction()
    {
        using var temp = new TemporaryWorkingDirectory();
        var state = new RecordingState();
        ImageRenameCliDependencies dependencies = CreateRecordingDependencies(state);
        TextWriter originalError = Console.Error;
        var error = new StringWriter();

        try
        {
            Console.SetError(error);

            await ImageRenameCli.RunAsync(["image.webp", "--naming", "unknown"], dependencies);

            Assert.That(Environment.ExitCode, Is.EqualTo(1));
            Assert.That(error.ToString(), Does.Contain("Unsupported naming convention"));
            Assert.That(state.RunnerWasCreated, Is.False);
        }
        finally
        {
            Console.SetError(originalError);
        }
    }

    private static ImageRenameCliDependencies CreateRecordingDependencies(RecordingState state)
    {
        return new ImageRenameCliDependencies
        {
            ModelFactory = preferences =>
            {
                state.ReceivedPreferences = preferences;
                return new RecordingModel();
            },
            RenamerFactory = (preferences, model) =>
            {
                state.RunnerWasCreated = true;
                state.ReceivedPreferences = preferences;
                return new RecordingRenamer();
            }
        };
    }

    private static ImageNamingPreferences LoadExpectedPreferences()
    {
        ImageRenameOptionParseResult parsed = new ImageRenameOptionsParser().Parse(["image.webp"]);
        ProjectLocalConfig? config = new ImageRenameConfigurationLoader().Load(parsed.Options!.ConfigPath);
        return new ImageNamingPreferenceResolver().Resolve(config);
    }

    private sealed class RecordingState
    {
        public ImageNamingPreferences? ReceivedPreferences { get; set; }

        public bool RunnerWasCreated { get; set; }
    }

    private sealed class RecordingModel : IForTalkingWithModel
    {
        public Task<ImageFile> GetNewImageNameAsync(ImageFile originalImageFile)
        {
            return Task.FromResult(originalImageFile);
        }
    }

    private sealed class RecordingRenamer : IForRenamingImage
    {
        public Task<string> RenameImageAsync(string imagePath)
        {
            return Task.FromResult(imagePath);
        }
    }
}
