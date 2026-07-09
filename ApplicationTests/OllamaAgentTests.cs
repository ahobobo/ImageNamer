using Application.Models;
using Application.Ports.Driven;
using Infrastructure.ForTalkingWithModels;

namespace ApplicationTests;

public class OllamaAgentTests
{
    [Test]
    public async Task GetNewImageNameAsync_SendsConvertedWebpPayloadMetadataToTransport()
    {
        ImageFile originalImage = new(
            "Bellwether_Zootopia.webp",
            ".webp",
            @"C:\images\Bellwether_Zootopia.webp",
            Convert.ToBase64String(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }),
            "image/png",
            true);
        var transport = new RecordingOllamaChatTransport("Bellwether - Zootopia!.webp");
        var sut = new OllamaAgent(transport);

        ImageFile renamedImage = await sut.GetNewImageNameAsync(originalImage);

        Assert.That(transport.Requests, Has.Count.EqualTo(1));
        Assert.That(transport.Requests[0].SystemInstructions, Is.EqualTo(OllamaAgent.Instructions));
        Assert.That(transport.Requests[0].Prompt, Is.EqualTo($"Original filename: {originalImage.Name}"));
        Assert.That(
            transport.Requests[0].Images,
            Is.EqualTo(new[] { new ModelImageContent(originalImage.Base64Content, originalImage.MimeType) }));
        Assert.That(originalImage.ModelPayloadWasConverted, Is.True);
        Assert.That(renamedImage.Name, Is.EqualTo("Bellwether - Zootopia!.webp"));
    }

    [Test]
    public async Task GetNewImageNameAsync_LeavesNonWebpPayloadMetadataUnchanged()
    {
        ImageFile originalImage = TestImageFile.Read("recursive\\nested\\wp8078607.jpg");
        var transport = new RecordingOllamaChatTransport("wp8078607.jpg");
        var sut = new OllamaAgent(transport);

        await sut.GetNewImageNameAsync(originalImage);

        Assert.That(transport.Requests, Has.Count.EqualTo(1));
        Assert.That(transport.Requests[0].Images, Has.Length.EqualTo(1));
        Assert.That(transport.Requests[0].Images[0].MimeType, Is.EqualTo("image/jpeg"));
        Assert.That(originalImage.ModelPayloadWasConverted, Is.False);
    }

    private sealed class RecordingOllamaChatTransport : IOllamaChatTransport
    {
        private readonly Queue<string> _responses;

        public RecordingOllamaChatTransport(params string[] responses)
        {
            _responses = new Queue<string>(responses);
        }

        public List<RecordedRequest> Requests { get; } = [];

        public IAsyncEnumerable<string> SendAsync(
            string systemInstructions,
            string prompt,
            IReadOnlyList<ModelImageContent> images)
        {
            Requests.Add(new RecordedRequest(systemInstructions, prompt, images.ToArray()));

            if (_responses.Count == 0)
            {
                throw new InvalidOperationException("No more fake responses were configured.");
            }

            string response = _responses.Dequeue();
            return StreamResponse(response);
        }

        private static async IAsyncEnumerable<string> StreamResponse(string response)
        {
            foreach (char character in response)
            {
                yield return character.ToString();
                await Task.Yield();
            }
        }
    }

    private sealed record RecordedRequest(
        string SystemInstructions,
        string Prompt,
        ModelImageContent[] Images);
}
