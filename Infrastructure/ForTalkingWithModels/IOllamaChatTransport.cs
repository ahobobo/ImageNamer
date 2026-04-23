using System.Collections.Generic;

namespace Infrastructure.ForTalkingWithModels
{
    public interface IOllamaChatTransport
    {
        IAsyncEnumerable<string> SendAsync(string systemInstructions, string prompt, IReadOnlyList<string> images);
    }
}
