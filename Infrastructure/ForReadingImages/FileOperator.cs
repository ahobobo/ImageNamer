using Application.Models;
using Application.Ports.Driven;

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
        string base64Content = Convert.ToBase64String(imageBytes);

        return new ImageFile(fileName, extension, path, base64Content);
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
}
