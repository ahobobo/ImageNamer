
namespace Application.Models
{
    public record class ImageFile(
        string Name,
        string Extension,
        string Path,
        string Base64Content,
        string MimeType);

    public record class ModelImageContent(string Base64Content, string MimeType);




}
