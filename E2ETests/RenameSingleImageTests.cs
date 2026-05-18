using TestsShared;

namespace E2ETests;

[TestFixture]
public class RenameSingleImageTests : E2ETestBase
{
    [Test]
    public async Task Rename_SingleImage_WithSnakeNaming_RenamesFileCorrectly()
    {
        // Arrange
        string imagePath = Path.Combine(TempDir.Path, "Bellwether_Zootopia.webp");
        Assert.That(File.Exists(imagePath), Is.True);

        // Act
        var (exitCode, output, error) = await CliDriver.RunAsync([imagePath, "--naming", "snake"]);

        // Assert
        Assert.That(exitCode, Is.EqualTo(0));
        Assert.That(output, Does.Contain("Renamed Bellwether_Zootopia.webp to"));
        
        // Verify file was renamed (original should be gone, new one should exist)
        Assert.That(File.Exists(imagePath), Is.False);
        
        // Find the new file
        var files = Directory.GetFiles(TempDir.Path, "*.webp");
        Assert.That(files, Has.Length.EqualTo(1));
        string newFile = Path.GetFileName(files[0]);
        Assert.That(newFile, Is.Not.EqualTo("Bellwether_Zootopia.webp"));
        Assert.That(newFile, Is.EqualTo(newFile.ToLowerInvariant()));
    }
}
