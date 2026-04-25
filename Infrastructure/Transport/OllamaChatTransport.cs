using Application.Ports.Driven;
using Infrastructure.ForTalkingWithModels;
using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Transport
{
    public sealed class OllamaChatTransport : IOllamaChatTransport
    {
        private readonly IChatClient _chatClient;

        public OllamaChatTransport(IChatClient chatClient)
        {
            _chatClient = chatClient;
        }

        public IAsyncEnumerable<string> SendAsync(string systemInstructions, string prompt, IReadOnlyList<string> images)
        {
            if (!string.Equals(systemInstructions, OllamaAgent.Instructions, StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Unexpected system instructions for Ollama chat.");
            }

            return SendInternalAsync(systemInstructions, prompt, images);
        }

        private async IAsyncEnumerable<string> SendInternalAsync(string systemInstructions, string prompt, IReadOnlyList<string> images)
        {
            var messages = new List<ChatMessage>
            {
                new ChatMessage(ChatRole.System, systemInstructions),
                new ChatMessage(ChatRole.User, prompt)
            };

            foreach (var imageBase64 in images)
            {
                byte[] bytes = Convert.FromBase64String(imageBase64);
                messages.Last().Contents.Add(new DataContent(bytes, "image/png"));
            }

            await foreach (var update in _chatClient.GetStreamingResponseAsync(messages))
            {
                if (update.Text != null)
                {
                    yield return update.Text;
                }
            }
        }
    }
}
