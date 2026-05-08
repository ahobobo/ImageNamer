using Application.Ports.Driven;
using Microsoft.Extensions.AI;

namespace Infrastructure.Transport;

public sealed class OllamaChatTransport : IOllamaChatTransport
{
    private readonly IChatClient _chatClient;

    public OllamaChatTransport(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public IAsyncEnumerable<string> SendAsync(string systemInstructions, string prompt, IReadOnlyList<string> images)
    {
        return SendInternalAsync(systemInstructions, prompt, images);
    }

    private async IAsyncEnumerable<string> SendInternalAsync(string systemInstructions, string prompt, IReadOnlyList<string> images)
    {
        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, systemInstructions),
            new(ChatRole.User, prompt)
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
