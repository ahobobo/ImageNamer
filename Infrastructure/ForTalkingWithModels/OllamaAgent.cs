using Application.Models;
using Application.Ports.Driven;
using OllamaSharp;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ForTalkingWithModels
{
    public class OllamaAgent : IForTalkingWithModel
    {
        private const string Instructions = """
            You are an image renamer assistant.
            Given an image filename and its content, propose a better, descriptive filename.
            The filename should be under 20 characters.
            If you recognize the character(s) in the image, include their name(s) in the filename at the beginning of the name.
            Return ONLY the new filename and nothing else.
            """;

        private readonly Chat _chat;

        public OllamaAgent()
        {
            var uri = new Uri("http://localhost:11434");
            var client = new OllamaApiClient(uri, defaultModel: "gemma4:e4b");
            _chat = new Chat(client, Instructions);
        }

        public async Task<ImageFile> GetNewImageNameAsync(ImageFile originalImageFile)
        {
            string prompt = $"Original filename: {originalImageFile.Name}";

            var response = _chat.SendAsync(prompt, [originalImageFile.Base64Content]);
            var generatedName = new StringBuilder();

            await foreach (var token in response)
            {
                generatedName.Append(token);
            }

            string newName = generatedName.ToString().Trim();

            if (!newName.EndsWith(originalImageFile.Extension, StringComparison.OrdinalIgnoreCase))
            {
                newName = Path.ChangeExtension(newName, originalImageFile.Extension);
            }

            return originalImageFile with { Name = newName };
        }
    }
}
