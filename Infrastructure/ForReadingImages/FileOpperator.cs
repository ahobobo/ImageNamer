using Application.Models;
using Application.Ports.Driven;
using System;
using System.IO;
using System.Linq;

namespace Infrastructure.ForReadingImages
{
    public class FileOpperator : IForInteractingWithFile
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

            // A basic check to see if it's a valid file by reading bytes
            byte[] imageBytes = File.ReadAllBytes(path);
            
            // Minimal header check (optional, but good for "verify that it is an image")
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
            
            string baseNewPath = Path.Combine(dirPath, $"{nameWithoutExtension}{extension}").ToLowerInvariant();
            string newPath = baseNewPath;

            // If the paths are exactly the same, nothing to do.
            if (string.Equals(originalFile.Path, newPath, StringComparison.Ordinal))
            {
                return originalFile.Path;
            }

            // Handle case-only renames on Windows (e.g., image.webp -> Image.webp)
            bool isSameFileDifferentCase = OperatingSystem.IsWindows() &&
                                           string.Equals(originalFile.Path, newPath, StringComparison.OrdinalIgnoreCase);

            if (!isSameFileDifferentCase)
            {
                int counter = 1;
                while (File.Exists(newPath))
                {
                    newPath = Path.Combine(dirPath, $"{nameWithoutExtension} {counter++}{extension}").ToLowerInvariant();
                }
            }

            if (isSameFileDifferentCase)
            {
                string tempPath = Path.Combine(
                    dirPath,
                    $".{Guid.NewGuid():N}{extension}");

                File.Move(originalFile.Path, tempPath);
                File.Move(tempPath, newPath);
                return newPath;
            }

            File.Move(originalFile.Path, newPath);
            return newPath;
        }
    }
}
