using Application.Models;
using Application.Ports.Driven;
using System;
using System.IO;
using System.Linq;

namespace Infrastructure.ForReadingImages
{
    public class FileOpperator : IForInteractingWithFile
    {
        private readonly string[] _supportedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };

        public ImageFile ReadFile(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Image file not found", path);
            }

            string extension = Path.GetExtension(path).ToLowerInvariant();
            if (!_supportedExtensions.Contains(extension))
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

        public void RenameFile(ImageFile originalFile, ImageFile renamedFile)
        {
            if (!File.Exists(originalFile.Path))
            {
                throw new FileNotFoundException("Source file not found", originalFile.Path);
            }

            string? directory = Path.GetDirectoryName(originalFile.Path);
            string newPath = Path.Combine(directory ?? string.Empty, renamedFile.Name);

            File.Move(originalFile.Path, newPath);
        }
    }
}
