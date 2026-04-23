using Application.Ports.Driven;
using System.Linq;

namespace Infrastructure.Validation
{
    public sealed class FileNameValidator : IForValidatingFileNames
    {
        public bool IsValidFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return false;
            }

            return fileName.All(IsAllowedNameCharacter);
        }

        private static bool IsAllowedNameCharacter(char character)
        {
            return char.IsLetterOrDigit(character) || character == ' ';
        }
    }
}
