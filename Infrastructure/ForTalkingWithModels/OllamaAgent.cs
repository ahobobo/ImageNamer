using Application.Models;
using Application.Ports.Driven;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ForTalkingWithModels
{
    public class OllamaAgent : IForTalkingWithModel
    {
        private const string Instructions = """
            You are an image renamer assistant. 
            Given an image filename and its content (if available), propose a better, descriptive filename. 
            The filename should be under 20 characters.
            If you recognize the character(s) in the image, include their name(s) in the filename at the begining of the name.
            Return ONLY the new filename nothing else.
            """;

        private readonly AIAgent _agent;

        public OllamaAgent()
        {
            // 1. Initialize the Ollama Chat Client with the local endpoint and model
            var chatClient = new OllamaChatClient(
                new Uri("http://localhost:11434"),
                modelId: "gemma4:e4b");

            // 2. Create the Agent from the chat client
            _agent = chatClient.AsAIAgent(instructions: Instructions);
        }

        public async Task<ImageFile> GetNewImageNameAsync(ImageFile originalImageFile)
        {
            string prompt = $"Original filename: {originalImageFile.Name}";

            // 1. Convert base64 string back to bytes
            byte[] imageBytes = Convert.FromBase64String(originalImageFile.Base64Content);

            // 2. Determine media type from extension
            string mediaType = GetMediaType(originalImageFile.Extension);

            // 3. Create a multimodal message using Microsoft.Agents.AI components
            var message = new ChatMessage(ChatRole.User, [
                new TextContent(prompt),
                new DataContent(imageBytes, mediaType)
            ]);

            // 4. Run the agent with the message
            var response = await _agent.RunAsync(message);
            
            string newName = response.Text.Trim();

            return originalImageFile with { Name = newName };
        }

        private static string GetMediaType(string extension)
        {
            return extension.ToLowerInvariant() switch
            {
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                ".bmp" => "image/bmp",
                _ => "application/octet-stream"
            };
        }
    }
}
