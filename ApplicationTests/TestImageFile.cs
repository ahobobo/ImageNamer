using Application.Models;
using TestsShared;

namespace ApplicationTests;

internal static class TestImageFile
{
    public static ImageFile Read(string fileName)
    {
        string testDataPath = Path.Combine(RepositoryPaths.TestDataDirectory, fileName);

        byte[] imageBytes = File.ReadAllBytes(testDataPath);
        string base64Content = Convert.ToBase64String(imageBytes);
        string extension = Path.GetExtension(testDataPath);

        return new ImageFile(fileName, extension, testDataPath, base64Content);
    }
}
