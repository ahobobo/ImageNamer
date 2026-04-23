using System.Collections.Generic;

namespace Application.Ports.Driven
{
    public interface IOllamaChatTransport
    {
        IAsyncEnumerable<string> SendAsync(string systemInstructions, string prompt, IReadOnlyList<string> images);
    }
}
