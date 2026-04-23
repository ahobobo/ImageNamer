namespace Infrastructure.ForReadingImages;

public static class ImageFileExtensions
{
    private static readonly HashSet<string> SupportedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".gif",
        ".bmp",
        ".webp"
    };

    public static bool IsSupportedExtension(string extension)
    {
        return SupportedExtensions.Contains(extension);
    }

    public static bool IsSupportedImagePath(string path)
    {
        return IsSupportedExtension(Path.GetExtension(path));
    }
}
