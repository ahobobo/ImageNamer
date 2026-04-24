using Application.Ports.Driving;
using ImageNamer.Cli;

namespace ApplicationTests;

public class ImageRenameRunnerTests
{
    [Test]
    public async Task RunAsync_WithFileInput_ProcessesSingleImage()
    {
        string root = CreateTempDirectory();
        string imagePath = Path.Combine(root, "single.jpg");
        await File.WriteAllTextAsync(imagePath, "data");

        var renamer = new RecordingRenamer();
        var runner = new ImageRenameRunner(renamer);

        try
        {
            ImageRenameRunResult result = await runner.RunAsync(imagePath, new StringWriter(), new StringWriter());

            Assert.That(result.SuccessfulCount, Is.EqualTo(1));
            Assert.That(result.FailureCount, Is.EqualTo(0));
            Assert.That(renamer.ReceivedPaths, Is.EqualTo(new[] { imagePath }));
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }

    [Test]
    public async Task RunAsync_WithDirectoryInput_ProcessesSupportedImagesRecursivelyAndSkipsUnsupportedFiles()
    {
        string root = CreateTempDirectory();
        string nested = Path.Combine(root, "nested");
        string deeper = Path.Combine(nested, "deeper");

        Directory.CreateDirectory(deeper);

        string firstImage = Path.Combine(root, "a.webp");
        string secondImage = Path.Combine(nested, "b.png");
        string thirdImage = Path.Combine(deeper, "c.gif");
        string skippedFile = Path.Combine(root, "notes.txt");

        await File.WriteAllTextAsync(firstImage, "data");
        await File.WriteAllTextAsync(secondImage, "data");
        await File.WriteAllTextAsync(thirdImage, "data");
        await File.WriteAllTextAsync(skippedFile, "ignore me");

        var renamer = new RecordingRenamer();
        var runner = new ImageRenameRunner(renamer);

        try
        {
            ImageRenameRunResult result = await runner.RunAsync(root, new StringWriter(), new StringWriter());

            Assert.That(result.SuccessfulCount, Is.EqualTo(3));
            Assert.That(result.FailureCount, Is.EqualTo(0));
            Assert.That(renamer.ReceivedPaths, Is.EqualTo(new[] { firstImage, secondImage, thirdImage }));
            Assert.That(renamer.ReceivedPaths, Does.Not.Contain(skippedFile));
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }

    [Test]
    public async Task RunAsync_ContinuesWhenOneImageRenameFails()
    {
        string root = CreateTempDirectory();
        string firstImage = Path.Combine(root, "a.jpg");
        string failingImage = Path.Combine(root, "b.jpg");
        string thirdImage = Path.Combine(root, "c.jpg");

        await File.WriteAllTextAsync(firstImage, "data");
        await File.WriteAllTextAsync(failingImage, "data");
        await File.WriteAllTextAsync(thirdImage, "data");

        var renamer = new RecordingRenamer(failingImage);
        var runner = new ImageRenameRunner(renamer);

        try
        {
            ImageRenameRunResult result = await runner.RunAsync(root, new StringWriter(), new StringWriter());

            Assert.That(result.SuccessfulCount, Is.EqualTo(2));
            Assert.That(result.FailureCount, Is.EqualTo(1));
            Assert.That(renamer.ReceivedPaths, Is.EqualTo(new[] { firstImage, failingImage, thirdImage }));
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }

    [Test]
    public void GetUsageText_ExplainsFileAndDirectoryModes()
    {
        string usage = ImageRenameCli.GetUsageText();

        Assert.That(usage, Does.Contain("<file_path>"));
        Assert.That(usage, Does.Contain("<directory_path>"));
    }

    private static string CreateTempDirectory()
    {
        string root = Path.Combine(Path.GetTempPath(), "ImageNamerTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        return root;
    }

    private sealed class RecordingRenamer : IForRenamingImage
    {
        private readonly string? _failingPath;

        public RecordingRenamer(string? failingPath = null)
        {
            _failingPath = failingPath;
        }

        public List<string> ReceivedPaths { get; } = [];

        public Task<string> RenameImageAsync(string imagePath)
        {
            ReceivedPaths.Add(imagePath);

            if (_failingPath is not null &&
                string.Equals(imagePath, _failingPath, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Rename failed.");
            }

            return Task.FromResult(imagePath);
        }
    }
}
