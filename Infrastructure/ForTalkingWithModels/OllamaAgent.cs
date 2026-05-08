using Application.Models;
using Application.Ports.Driven;
using System.Text;

namespace Infrastructure.ForTalkingWithModels;

public class OllamaAgent : IForTalkingWithModel
{
    public const string Instructions = """
        You are an image renamer assistant.
        Given an image filename and its content, propose a better, descriptive filename.
        The filename should be descriptive and contain multiple keywords to make it easily searchable.
        The filename should be at least 3 words long, but ideally 5 or more words.
        Be creative and descriptive, but do not add any extra information that is not in the image.
        If you recognize the character(s) in the image, include their name(s) in the filename at the beginning of the name.
        Return only the new filename and nothing else.
        """;

    private readonly IOllamaChatTransport _chatTransport;

    public OllamaAgent(IOllamaChatTransport chatTransport, IForValidatingFileNames fileNameValidator)
    {
        _chatTransport = chatTransport;
    }

    public async Task<ImageFile> GetNewImageNameAsync(ImageFile originalImageFile)
    {
        string response = await GetRawGeneratedNameAsync(
            originalImageFile,
            BuildInitialPrompt(originalImageFile));

        string normalizedName = NormalizeGeneratedName(response, originalImageFile.Extension);

        if (string.IsNullOrWhiteSpace(Path.GetFileNameWithoutExtension(normalizedName)))
        {
            throw new InvalidDataException("The model returned an empty filename.");
        }

        return originalImageFile with { Name = normalizedName };
    }

    private static string BuildInitialPrompt(ImageFile originalImageFile)
    {
        return $"Original filename: {originalImageFile.Name}";
    }

    private async Task<string> GetRawGeneratedNameAsync(ImageFile originalImageFile, string prompt)
    {
        var response = _chatTransport.SendAsync(Instructions, prompt, [originalImageFile.Base64Content]);
        var generatedName = new StringBuilder();

        await foreach (var token in response)
        {
            generatedName.Append(token);
        }

        return generatedName.ToString().Trim();
    }

    private static string NormalizeGeneratedName(string generatedName, string extension)
    {
        string normalizedExtension = extension.StartsWith('.') ? extension : $".{extension}";
        string nameWithoutExtension = RemoveExpectedExtension(generatedName, normalizedExtension);
        return Path.ChangeExtension(nameWithoutExtension, normalizedExtension);
    }

    private static string RemoveExpectedExtension(string generatedName, string extension)
    {
        return generatedName.EndsWith(extension, StringComparison.OrdinalIgnoreCase)
            ? generatedName[..^extension.Length].TrimEnd()
            : generatedName;
    }
}
