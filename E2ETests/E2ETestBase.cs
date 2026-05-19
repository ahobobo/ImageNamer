using TestsShared;

namespace E2ETests;

public abstract class E2ETestBase
{
    protected TemporaryWorkingDirectory TempDir { get; private set; } = null!;
    protected string TestDataPath { get; private set; } = null!;

    [SetUp]
    public void SetUp()
    {
        TempDir = new TemporaryWorkingDirectory();

        // Find project root by looking for ImageNamer.slnx
        string current = AppContext.BaseDirectory;
        while (!File.Exists(Path.Combine(current, "ImageNamer.slnx")))
        {
            current = Path.GetDirectoryName(current) ?? throw new Exception("Could not find project root.");
        }
        
        TestDataPath = Path.Combine(current, "TestData");
        
        // Copy TestData to TempDir
        CopyDirectory(TestDataPath, TempDir.Path);
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
