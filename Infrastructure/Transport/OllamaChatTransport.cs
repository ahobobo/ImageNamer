using Application.Ports.Driven;
using Application.Models;
using Microsoft.Extensions.AI;

namespace Infrastructure.Transport;

public sealed class OllamaChatTransport : IOllamaChatTransport
{
    private readonly IChatClient _chatClient;

    public OllamaChatTransport(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public IAsyncEnumerable<string> SendAsync(
        string systemInstructions,
        string prompt,
        IReadOnlyList<ModelImageContent> images)
    {
        return SendInternalAsync(systemInstructions, prompt, images);
    }

    private async IAsyncEnumerable<string> SendInternalAsync(
        string systemInstructions,
        string prompt,
        IReadOnlyList<ModelImageContent> images)
    {
        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, systemInstructions),
            new(ChatRole.User, prompt)
        };

        foreach (var image in images)
        {
            byte[] bytes = Convert.FromBase64String(image.Base64Content);
            messages.Last().Contents.Add(new DataContent(bytes.AsMemory(), image.MimeType));
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
