using Infrastructure.ForTalkingWithModels;
using Infrastructure.Transport;
using Infrastructure.Validation;
using Microsoft.Extensions.AI;
using OllamaSharp;

namespace ImageNamer.Cli.Factories
{
    public static class OllamaAgentFactory
    {
        public static OllamaAgent Create()
        {
            var uri = new Uri("http://localhost:11434");
            IChatClient client = new OllamaApiClient(uri, defaultModel: "gemma4:e4b");

            return new OllamaAgent(
                new OllamaChatTransport(client),
                new FileNameValidator());
        }
    }
}
