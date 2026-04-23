using Infrastructure.Transport;
using System;

namespace ApplicationTests;

public class OllamaChatTransportTests
{
    [Test]
    public void SendAsync_WhenSystemInstructionsDoNotMatch_Throws()
    {
        var sut = new OllamaChatTransport(null!);

        Assert.That(
            () => sut.SendAsync("wrong instructions", "prompt", Array.Empty<string>()),
            Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo("Unexpected system instructions for Ollama chat."));
    }
}
