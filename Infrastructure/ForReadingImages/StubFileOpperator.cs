using Application.Models;
using Application.Ports.Driven;
using System;
using System.IO;

namespace Infrastructure.ForReadingImages
{
    public class StubFileOpperator : IForInteractingWithFile
    {
        public ImageFile ReadFile(string path)
        {
            // We use a relative path that works from the project root or when running tests.
            // In a real stub, we might want more robust path resolution.
            string fileName = "Bellwether_Zootopia.webp";
            string testDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", fileName);

            if (!File.Exists(testDataPath))
            {
                // Try to find it in the current directory if BaseDirectory doesn't have it
                testDataPath = Path.Combine(Directory.GetCurrentDirectory(), "TestData", fileName);
            }

            if (!File.Exists(testDataPath))
            {
                // Last resort for typical .NET build output structure
                testDataPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "TestData", fileName);
            }

            byte[] imageBytes = File.ReadAllBytes(testDataPath);
            string base64Content = Convert.ToBase64String(imageBytes);
            string extension = Path.GetExtension(testDataPath);

            return new ImageFile(fileName, extension, testDataPath, base64Content);
        }

        public void RenameFile(ImageFile originalFile, ImageFile renamedFile)
        {
            // Stub implementation: do nothing
        }
    }
}
