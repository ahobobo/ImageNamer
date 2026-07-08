using Application.Models;
using Infrastructure.Transport;

namespace ApplicationTests;

public class OllamaChatTransportTests
{
    [Test]
    public void SendAsync_WhenSystemInstructionsDoNotMatch_DoesNotThrowBeforeEnumeration()
    {
        var sut = new OllamaChatTransport(null!);

        Assert.That(
            () => sut.SendAsync("custom instructions", "prompt", Array.Empty<ModelImageContent>()),
            Throws.Nothing);
    }
}
