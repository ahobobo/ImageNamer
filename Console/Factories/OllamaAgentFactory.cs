using Application.Models;
using Infrastructure.ForTalkingWithModels;
using Infrastructure.Transport;
using Microsoft.Extensions.AI;
using OllamaSharp;

namespace ImageNamer.Cli.Factories;

public static class OllamaAgentFactory
{
    public static OllamaAgent Create(ImageNamingPreferences preferences)
    {
        return Create(preferences.ModelName);
    }

    public static OllamaAgent Create(
        string modelName = ImageNamingPreferences.DefaultModelName,
        Func<Uri, string, IChatClient>? chatClientFactory = null)
    {
        var uri = new Uri("http://localhost:11434");
        chatClientFactory ??= static (endpoint, model) => new OllamaApiClient(endpoint, defaultModel: model);

        IChatClient client = chatClientFactory(uri, modelName);

        return new OllamaAgent(new OllamaChatTransport(client));
    }
}
