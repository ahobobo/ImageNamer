using ImageNamer.Cli.Factories;
using Microsoft.Extensions.AI;

namespace ApplicationTests;

public class OllamaAgentFactoryTests
{
    [Test]
    public void Create_PassesConfiguredModelNameToClientFactory()
    {
        string? receivedModel = null;

        OllamaAgentFactory.Create(
            "llava:latest",
            (_, model) =>
            {
                receivedModel = model;
                return null!;
            });

        Assert.That(receivedModel, Is.EqualTo("llava:latest"));
    }
}
