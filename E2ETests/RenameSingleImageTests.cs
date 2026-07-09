using TestsShared;

namespace E2ETests;

[TestFixture]
public class RenameSingleImageTests : E2ETestBase
{
    [Test]
    public async Task Rename_SingleWebpImage_WithSnakeNaming_RenamesFileCorrectlyAndSendsPngPayload()
    {
        // Arrange
        string imagePath = Path.Combine(TempDir.Path, "Bellwether_Zootopia.webp");
        var model = new CliDriver.DeterministicModel();
        Assert.That(File.Exists(imagePath), Is.True);

        // Act
        var (exitCode, output, error) = await CliDriver.RunAsync([imagePath, "--naming", "snake"], model);

        // Assert
        Assert.That(exitCode, Is.EqualTo(0));
        Assert.That(output, Does.Contain("Renamed Bellwether_Zootopia.webp to"));
        Assert.That(model.ReceivedImages, Has.Count.EqualTo(1));
        Assert.That(model.ReceivedImages[0].MimeType, Is.EqualTo("image/png"));
        Assert.That(model.ReceivedImages[0].Extension, Is.EqualTo(".webp"));
        Assert.That(model.ReceivedImages[0].ModelPayloadWasConverted, Is.True);
        
        // Verify file was renamed (original should be gone, new one should exist)
        Assert.That(File.Exists(imagePath), Is.False);
        
        // Find the new file
        var files = Directory.GetFiles(TempDir.Path, "*.webp");
        Assert.That(files, Has.Length.EqualTo(1));
        string newFile = Path.GetFileName(files[0]);
        Assert.That(newFile, Is.Not.EqualTo("Bellwether_Zootopia.webp"));
        Assert.That(newFile, Is.EqualTo(newFile.ToLowerInvariant()));
    }

    [Test]
    public async Task Rename_SingleInvalidWebp_ExitsNonZeroAndLeavesSourceFileUntouched()
    {
        string imagePath = Path.Combine(TempDir.Path, "InvalidImage.webp");
        File.Copy(Path.Combine(RepositoryPaths.TestDataDirectory, "InvalidImage.webp"), imagePath);

        var (exitCode, output, error) = await CliDriver.RunAsync([imagePath]);

        Assert.That(exitCode, Is.EqualTo(1));
        Assert.That(error, Does.Contain("WEBP"));
        Assert.That(File.Exists(imagePath), Is.True);
        Assert.That(output, Does.Not.Contain("Renamed InvalidImage.webp"));
    }
}
