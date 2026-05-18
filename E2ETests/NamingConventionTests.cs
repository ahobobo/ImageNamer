using TestsShared;

namespace E2ETests;

[TestFixture]
public class NamingConventionTests : E2ETestBase
{
    [TestCase("snake", @"^[a-z0-9_]+\.[a-z]+$")]
    [TestCase("kebab", @"^[a-z0-9-]+\.[a-z]+$")]
    [TestCase("pascal", @"^[A-Z][a-zA-Z0-9]*\.[a-z]+$")]
    [Test]
    public async Task Rename_WithNamingConvention_RespectsFormat(string convention, string regexPattern)
    {
        // Arrange
        string imagePath = Path.GetFullPath(Path.Combine(TempDir.Path, "Bellwether_Zootopia.webp"));

        // Act
        var (exitCode, output, error) = await CliDriver.RunAsync([imagePath, "--naming", convention]);

        // Assert
        Assert.That(exitCode, Is.EqualTo(0));
        
        var files = Directory.GetFiles(TempDir.Path, "*.webp");
        Assert.That(files, Has.Length.EqualTo(1));
        string newFile = Path.GetFileName(files[0]);
        
        Assert.That(newFile, Does.Match(regexPattern));
    }
}
