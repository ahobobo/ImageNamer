using TestsShared;

namespace E2ETests;

[TestFixture]
public class RecursiveRenamingTests : E2ETestBase
{
    [Test]
    public async Task Rename_DirectoryRecursively_RenamesAllImages()
    {
        // Act
        var (exitCode, output, error) = await CliDriver.RunAsync([TempDir.Path]);

        // Assert
        Assert.That(exitCode, Is.EqualTo(0));
        
        // Root image
        Assert.That(output, Does.Contain("Renamed Bellwether_Zootopia.webp to"));
        
        // Recursive images
        Assert.That(output, Does.Contain("Renamed 28c7427476e12347a1a586261cdece57.png to"));
        Assert.That(output, Does.Contain("Renamed wp8078607.jpg to"));

        // Verify files exist in their respective directories
        Assert.That(Directory.GetFiles(TempDir.Path, "*.webp"), Has.Length.EqualTo(1));
        
        string recursivePath = Path.Combine(TempDir.Path, "recursive");
        Assert.That(Directory.GetFiles(recursivePath, "*.png"), Has.Length.EqualTo(1));
        
        string nestedPath = Path.Combine(recursivePath, "nested");
        Assert.That(Directory.GetFiles(nestedPath, "*.jpg"), Has.Length.EqualTo(1));
    }
}
