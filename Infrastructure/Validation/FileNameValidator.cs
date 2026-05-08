using Application.Ports.Driven;

namespace Infrastructure.Validation;

public sealed class FileNameValidator : IForValidatingFileNames
{
    public bool IsValidFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return false;
        }

        HashSet<char> invalidCharacters = Path.GetInvalidFileNameChars().ToHashSet();
        return fileName.All(character => !invalidCharacters.Contains(character));
    }
}
