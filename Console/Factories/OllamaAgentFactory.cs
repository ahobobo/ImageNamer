using Infrastructure.ForTalkingWithModels;
using Infrastructure.Transport;
using Infrastructure.Validation;
using OllamaSharp;

namespace ImageNamer.Cli.Factories
{
    public static class OllamaAgentFactory
    {
        public static OllamaAgent Create()
        {
            var uri = new Uri("http://localhost:11434");
            var client = new OllamaApiClient(uri, defaultModel: "gemma4:e4b");
            var chat = new Chat(client, OllamaAgent.Instructions);

            return new OllamaAgent(
                new OllamaChatTransport(chat),
                new FileNameValidator());
        }
    }
}
