using TestsShared;

namespace E2ETests;

public abstract class E2ETestBase
{
    protected TemporaryWorkingDirectory TempDir { get; private set; } = null!;

    [SetUp]
    public void SetUp()
    {
        TempDir = new TemporaryWorkingDirectory();
        CopyDirectory(RepositoryPaths.TestDataDirectory, TempDir.Path);
        string invalidFixturePath = Path.Combine(TempDir.Path, "InvalidImage.webp");
        if (File.Exists(invalidFixturePath))
        {
            File.Delete(invalidFixturePath);
        }
    }

    private static void CopyDirectory(string sourceDir, string destinationDir)
    {
        var dir = new DirectoryInfo(sourceDir);
        if (!dir.Exists) throw new DirectoryNotFoundException($"Source directory not found: {sourceDir}");

        Directory.CreateDirectory(destinationDir);

        foreach (FileInfo file in dir.GetFiles())
        {
            string targetFilePath = Path.Combine(destinationDir, file.Name);
            file.CopyTo(targetFilePath);
        }

        foreach (DirectoryInfo subDir in dir.GetDirectories())
        {
            string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
            CopyDirectory(subDir.FullName, newDestinationDir);
        }
    }

    [TearDown]
    public void TearDown()
    {
        TempDir?.Dispose();
    }
}
