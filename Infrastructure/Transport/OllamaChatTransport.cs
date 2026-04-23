using Application.Ports.Driven;
using Infrastructure.ForTalkingWithModels;
using OllamaSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Transport
{
    public sealed class OllamaChatTransport : IOllamaChatTransport
    {
        private readonly Chat _chat;

        public OllamaChatTransport(Chat chat)
        {
            _chat = chat;
        }

        public IAsyncEnumerable<string> SendAsync(string systemInstructions, string prompt, IReadOnlyList<string> images)
        {
            if (!string.Equals(systemInstructions, OllamaAgent.Instructions, StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Unexpected system instructions for Ollama chat.");
            }

            return _chat.SendAsync(prompt, images.ToArray());
        }
    }
}
