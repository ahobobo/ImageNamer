using Application.Models;
using TestsShared;

namespace ApplicationTests;

internal static class TestImageFile
{
    public static string GetPath(string fileName)
    {
        return Path.Combine(RepositoryPaths.TestDataDirectory, fileName);
    }

    public static ImageFile Read(string fileName)
    {
        string testDataPath = GetPath(fileName);

        byte[] imageBytes = File.ReadAllBytes(testDataPath);
        string base64Content = Convert.ToBase64String(imageBytes);
        string extension = Path.GetExtension(testDataPath);
        string mimeType = extension.ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };

        return new ImageFile(fileName, extension, testDataPath, base64Content, mimeType);
    }
}
