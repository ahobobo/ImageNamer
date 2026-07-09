using Application.Models;
using Application.Ports.Driven;
using SkiaSharp;

namespace Infrastructure.ForReadingImages;

public class FileOperator : IForInteractingWithFile
{
    public ImageFile ReadFile(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("Image file not found", path);
        }

        string extension = Path.GetExtension(path).ToLowerInvariant();
        if (!ImageFileExtensions.IsSupportedExtension(extension))
        {
            throw new NotSupportedException($"File extension '{extension}' is not supported as an image.");
        }

        byte[] imageBytes = File.ReadAllBytes(path);

        if (imageBytes.Length < 4)
        {
            throw new InvalidDataException("File is too small to be a valid image.");
        }

        string fileName = Path.GetFileName(path);
        var preparedPayload = PrepareModelPayload(imageBytes, extension);
        string base64Content = Convert.ToBase64String(preparedPayload.Bytes);

        return new ImageFile(
            fileName,
            extension,
            path,
            base64Content,
            preparedPayload.MimeType,
            preparedPayload.WasConverted);
    }

    public string RenameFile(ImageFile originalFile, ImageFile renamedFile)
    {
        if (!File.Exists(originalFile.Path))
        {
            throw new FileNotFoundException("Source file not found", originalFile.Path);
        }

        string? directory = Path.GetDirectoryName(originalFile.Path);
        string dirPath = directory ?? string.Empty;
        string extension = renamedFile.Extension.ToLowerInvariant();
        string nameWithoutExtension = Path.GetFileNameWithoutExtension(renamedFile.Name);

        string baseNewPath = Path.Combine(dirPath, $"{nameWithoutExtension}{extension}");
        string newPath = baseNewPath;

        if (string.Equals(originalFile.Path, newPath, StringComparison.Ordinal))
        {
            return originalFile.Path;
        }

        bool isSameFileDifferentCase = OperatingSystem.IsWindows()
            && string.Equals(originalFile.Path, newPath, StringComparison.OrdinalIgnoreCase);

        if (!isSameFileDifferentCase)
        {
            int counter = 1;
            while (File.Exists(newPath))
            {
                newPath = Path.Combine(dirPath, $"{nameWithoutExtension} {counter++}{extension}");
            }
        }

        if (isSameFileDifferentCase)
        {
            string tempPath = Path.Combine(dirPath, $".{Guid.NewGuid():N}{extension}");

            File.Move(originalFile.Path, tempPath);
            File.Move(tempPath, newPath);
            return newPath;
        }

        File.Move(originalFile.Path, newPath);
        return newPath;
    }

    private static string GetMimeType(string extension)
    {
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".webp" => "image/webp",
            _ => throw new NotSupportedException($"File extension '{extension}' is not supported as an image.")
        };
    }

    private static PreparedPayload PrepareModelPayload(byte[] imageBytes, string extension)
    {
        if (!string.Equals(extension, ".webp", StringComparison.OrdinalIgnoreCase))
        {
            return new PreparedPayload(imageBytes, GetMimeType(extension), false);
        }

        SKBitmap? bitmap;

        try
        {
            bitmap = SKBitmap.Decode(imageBytes);
        }
        catch (ArgumentNullException ex)
        {
            throw new InvalidDataException("WEBP image could not be decoded for model compatibility.", ex);
        }

        if (bitmap is null)
        {
            throw new InvalidDataException("WEBP image could not be decoded for model compatibility.");
        }

        using (bitmap)
        {
            using SKData? encodedImage = bitmap.Encode(SKEncodedImageFormat.Png, quality: 100);
            if (encodedImage is null)
            {
                throw new InvalidDataException("WEBP image could not be encoded as PNG for model compatibility.");
            }

            return new PreparedPayload(encodedImage.ToArray(), "image/png", true);
        }
    }

    private sealed record PreparedPayload(byte[] Bytes, string MimeType, bool WasConverted);
}
