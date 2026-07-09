
namespace Application.Models
{
    public record class ImageFile(
        string Name,
        string Extension,
        string Path,
        string Base64Content,
        string MimeType,
        bool ModelPayloadWasConverted = false);

    public record class ModelImageContent(string Base64Content, string MimeType);
}
