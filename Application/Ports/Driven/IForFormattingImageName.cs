using Application.Models;

namespace Application.Ports.Driven;

public interface IForFormattingImageName
{
    string FormatName(string generatedName, string extension, ImageNamingPreferences preferences);
}
