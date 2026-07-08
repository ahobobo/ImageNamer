using Application.Models;
using Infrastructure.ForReadingImages;
using TestsShared;

namespace ApplicationTests;

public class FileOperatorTests
{
    [Test]
    public void RenameFile_UsesRequestedFinalFilenameWithoutForcingLowercase()
    {
        using var temp = new TemporaryDirectory();
        string sourcePath = Path.Combine(temp.Path, "original.webp");
        string expectedPath = Path.Combine(temp.Path, "NewName.webp");

        File.WriteAllBytes(sourcePath, [1, 2, 3, 4]);

        var original = new ImageFile("original.webp", ".webp", sourcePath, string.Empty, "image/webp");
        var renamed = new ImageFile("NewName.webp", ".webp", sourcePath, string.Empty, "image/webp");
        var sut = new FileOperator();

        sut.RenameFile(original, renamed);

        Assert.That(File.Exists(expectedPath), Is.True);
        Assert.That(File.Exists(sourcePath), Is.False);
    }

    [Test]
    public void RenameFile_LeavesSamePathInPlace()
    {
        using var temp = new TemporaryDirectory();
        string sourcePath = Path.Combine(temp.Path, "alreadylower.webp");

        File.WriteAllBytes(sourcePath, [1, 2, 3, 4]);

        var original = new ImageFile("alreadylower.webp", ".webp", sourcePath, string.Empty, "image/webp");
        var renamed = new ImageFile("alreadylower.webp", ".webp", sourcePath, string.Empty, "image/webp");
        var sut = new FileOperator();

        sut.RenameFile(original, renamed);

        Assert.That(File.Exists(sourcePath), Is.True);
    }

    [Test]
    public void RenameFile_HandlesCollisionsByAppendingNumber()
    {
        using var temp = new TemporaryDirectory();
        string sourcePath = Path.Combine(temp.Path, "image.webp");
        string existingPath1 = Path.Combine(temp.Path, "newname.webp");
        string existingPath2 = Path.Combine(temp.Path, "newname 1.webp");
        string expectedPath = Path.Combine(temp.Path, "newname 2.webp");

        File.WriteAllBytes(sourcePath, [1, 2, 3, 4]);
        File.WriteAllBytes(existingPath1, [5, 6, 7, 8]);
        File.WriteAllBytes(existingPath2, [9, 10, 11, 12]);

        var original = new ImageFile("image.webp", ".webp", sourcePath, string.Empty, "image/webp");
        var renamed = new ImageFile("newname.webp", ".webp", sourcePath, string.Empty, "image/webp");
        var sut = new FileOperator();

        sut.RenameFile(original, renamed);

        Assert.That(File.Exists(expectedPath), Is.True);
        Assert.That(File.Exists(sourcePath), Is.False);
        Assert.That(File.Exists(existingPath1), Is.True);
        Assert.That(File.Exists(existingPath2), Is.True);
    }

    [TestCase("image.webp", "image/webp")]
    [TestCase("image.jpg", "image/jpeg")]
    [TestCase("image.jpeg", "image/jpeg")]
    [TestCase("image.png", "image/png")]
    [TestCase("image.gif", "image/gif")]
    [TestCase("image.bmp", "image/bmp")]
    public void ReadFile_SetsMimeTypeFromExtension(string fileName, string expectedMimeType)
    {
        using var temp = new TemporaryDirectory();
        string sourcePath = Path.Combine(temp.Path, fileName);
        File.WriteAllBytes(sourcePath, [1, 2, 3, 4]);
        var sut = new FileOperator();

        ImageFile result = sut.ReadFile(sourcePath);

        Assert.That(result.MimeType, Is.EqualTo(expectedMimeType));
    }
}
