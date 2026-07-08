using System.Collections.Generic;
using Application.Models;

namespace Application.Ports.Driven
{
    public interface IOllamaChatTransport
    {
        IAsyncEnumerable<string> SendAsync(
            string systemInstructions,
            string prompt,
            IReadOnlyList<ModelImageContent> images);
    }
}
