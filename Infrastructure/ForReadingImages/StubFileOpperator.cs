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
            string fileName = "Bellwether_Zootopia.webp";
            string testDataPath = ResolveTestDataPath(fileName);

            byte[] imageBytes = File.ReadAllBytes(testDataPath);
            string base64Content = Convert.ToBase64String(imageBytes);
            string extension = Path.GetExtension(testDataPath);

            return new ImageFile(fileName, extension, testDataPath, base64Content);
        }

        public string RenameFile(ImageFile originalFile, ImageFile renamedFile)
        {
            // Stub implementation: do nothing
            return originalFile.Path;
        }

        private static string ResolveTestDataPath(string fileName)
        {
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            for (int i = 0; i < 8; i++)
            {
                string candidatePath = Path.Combine(currentDirectory, "TestData", fileName);
                if (File.Exists(candidatePath))
                {
                    return candidatePath;
                }

                DirectoryInfo? parent = Directory.GetParent(currentDirectory);
                if (parent is null)
                {
                    break;
                }

                currentDirectory = parent.FullName;
            }

            throw new FileNotFoundException($"Could not locate TestData/{fileName} from {AppDomain.CurrentDomain.BaseDirectory}.");
        }
    }
}
