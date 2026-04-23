using Application.Models;
using Infrastructure.ForReadingImages;

namespace ApplicationTests;

public class FileOpperatorTests
{
    [Test]
    public void RenameFile_LowercasesTheFinalFilename()
    {
        string root = CreateTempDirectory();
        string sourcePath = Path.Combine(root, "MiXeDName.WEBP");
        string expectedPath = Path.Combine(root, "newname.webp");

        File.WriteAllBytes(sourcePath, [1, 2, 3, 4]);

        var original = new ImageFile("MiXeDName.WEBP", ".WEBP", sourcePath, string.Empty);
        var renamed = new ImageFile("NeWNaMe.WEBP", ".WEBP", sourcePath, string.Empty);
        var sut = new FileOpperator();

        try
        {
            sut.RenameFile(original, renamed);

            Assert.That(File.Exists(expectedPath), Is.True);
            Assert.That(File.Exists(sourcePath), Is.False);
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }

    [Test]
    public void RenameFile_LeavesAlreadyLowercaseNameInPlace()
    {
        string root = CreateTempDirectory();
        string sourcePath = Path.Combine(root, "alreadylower.webp");

        File.WriteAllBytes(sourcePath, [1, 2, 3, 4]);

        var original = new ImageFile("alreadylower.webp", ".webp", sourcePath, string.Empty);
        var renamed = new ImageFile("alreadylower.webp", ".webp", sourcePath, string.Empty);
        var sut = new FileOpperator();

        try
        {
            sut.RenameFile(original, renamed);

            Assert.That(File.Exists(sourcePath), Is.True);
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }

    private static string CreateTempDirectory()
    {
        string root = Path.Combine(Path.GetTempPath(), "ImageNamerTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        return root;
    }
}
