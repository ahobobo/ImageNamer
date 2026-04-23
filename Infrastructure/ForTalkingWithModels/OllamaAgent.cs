using Application.Models;
using Application.Ports.Driven;
using OllamaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ForTalkingWithModels
{
    public class OllamaAgent : IForTalkingWithModel
    {
        public const string Instructions = """
            You are an image renamer assistant.
            Given an image filename and its content, propose a better, descriptive filename.
            The filename should be under 20 characters.
            Be creative and descriptive, but do not add any extra information that is not in the image.
            If you recognize the character(s) in the image, include their name(s) in the filename at the beginning of the name.
            Do **not** use punctuation in the filename at all. Only letters, numbers and spaces.
            Return **only** the new filename and nothing else.
            """;

        private const string RemovePunctuationRetryPrompt = """
            The previous filename contained punctuation.
            Remove all punctuation and return only letters, numbers, and spaces.
            Return only the filename and nothing else.
            """;

        private readonly IOllamaChatTransport _chatTransport;

        public OllamaAgent()
        {
            var uri = new Uri("http://localhost:11434");
            var client = new OllamaApiClient(uri, defaultModel: "gemma4:e4b");
            _chatTransport = new OllamaChatTransport(new Chat(client, Instructions));
        }

        public OllamaAgent(IOllamaChatTransport chatTransport)
        {
            _chatTransport = chatTransport;
        }

        public async Task<ImageFile> GetNewImageNameAsync(ImageFile originalImageFile)
        {
            string firstResponse = await GetRawGeneratedNameAsync(
                originalImageFile,
                BuildInitialPrompt(originalImageFile));

            if (!IsValidGeneratedName(firstResponse, originalImageFile.Extension))
            {
                string retryResponse = await GetRawGeneratedNameAsync(
                    originalImageFile,
                    BuildRetryPrompt(firstResponse));

                if (!IsValidGeneratedName(retryResponse, originalImageFile.Extension))
                {
                    throw new InvalidDataException("The model returned a filename with punctuation after retry.");
                }

                firstResponse = retryResponse;
            }

            string normalizedName = NormalizeGeneratedName(firstResponse, originalImageFile.Extension);

            return originalImageFile with { Name = normalizedName };
        }

        private static string BuildInitialPrompt(ImageFile originalImageFile)
        {
            return $"Original filename: {originalImageFile.Name}";
        }

        private static string BuildRetryPrompt(string previousResponse)
        {
            return $"""
                {RemovePunctuationRetryPrompt}
                Previous filename: {previousResponse}
                """;
        }

        private async Task<string> GetRawGeneratedNameAsync(ImageFile originalImageFile, string prompt)
        {
            var response = _chatTransport.SendAsync(Instructions, prompt, [originalImageFile.Base64Content]);
            var generatedName = new StringBuilder();

            await foreach (var token in response)
            {
                generatedName.Append(token);
            }

            return generatedName.ToString().Trim();
        }

        private static bool IsValidGeneratedName(string generatedName, string extension)
        {
            string nameWithoutExtension = RemoveExpectedExtension(generatedName, extension);

            if (string.IsNullOrWhiteSpace(nameWithoutExtension))
            {
                return false;
            }

            return nameWithoutExtension.All(IsAllowedNameCharacter);
        }

        private static string NormalizeGeneratedName(string generatedName, string extension)
        {
            string nameWithoutExtension = RemoveExpectedExtension(generatedName, extension);
            return Path.ChangeExtension(nameWithoutExtension, extension);
        }

        private static string RemoveExpectedExtension(string generatedName, string extension)
        {
            if (generatedName.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
            {
                return generatedName[..^extension.Length].TrimEnd();
            }

            return generatedName;
        }

        private static bool IsAllowedNameCharacter(char character)
        {
            return char.IsLetterOrDigit(character) || character == ' ';
        }

        private sealed class OllamaChatTransport : IOllamaChatTransport
        {
            private readonly Chat _chat;

            public OllamaChatTransport(Chat chat)
            {
                _chat = chat;
            }

            public IAsyncEnumerable<string> SendAsync(string systemInstructions, string prompt, IReadOnlyList<string> images)
            {
                if (!string.Equals(systemInstructions, Instructions, StringComparison.Ordinal))
                {
                    throw new InvalidOperationException("Unexpected system instructions for Ollama chat.");
                }

                return _chat.SendAsync(prompt, images.ToArray());
            }
        }
    }
}
