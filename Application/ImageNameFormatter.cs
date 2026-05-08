using Application.Models;
using Application.Ports.Driven;
using System.Text;

namespace Application;

public sealed class ImageNameFormatter : IForFormattingImageName
{
    public string FormatName(string generatedName, string extension, ImageNamingPreferences preferences)
    {
        if (string.IsNullOrWhiteSpace(generatedName))
        {
            throw new ArgumentException("Generated name must not be empty.", nameof(generatedName));
        }

        ImageNamingPreferenceResolver.Validate(preferences);

        string normalizedExtension = extension.StartsWith('.')
            ? extension.ToLowerInvariant()
            : $".{extension.ToLowerInvariant()}";

        string stemInput = RemoveExpectedExtension(generatedName.Trim(), normalizedExtension);
        string stem = preferences.NamingConvention switch
        {
            NamingConvention.Normal => FormatNormal(stemInput),
            NamingConvention.Snake => JoinWords(stemInput, "_", lower: true),
            NamingConvention.Capitalized => JoinWords(stemInput, " ", lower: false),
            NamingConvention.Pascal => JoinWords(stemInput, string.Empty, lower: false),
            NamingConvention.Kebab => JoinWords(stemInput, "-", lower: true),
            _ => throw new ArgumentException("Naming convention is not supported.", nameof(preferences))
        };

        stem = EnforceMaxLength(stem, preferences.MaxNameLength);

        if (string.IsNullOrWhiteSpace(stem))
        {
            throw new InvalidOperationException("Generated name could not be converted into a valid filename.");
        }

        return $"{stem}{normalizedExtension}";
    }

    private static string FormatNormal(string value)
    {
        HashSet<char> invalid = Path.GetInvalidFileNameChars().ToHashSet();
        var builder = new StringBuilder();
        bool previousWasSpace = false;

        foreach (char character in value)
        {
            if (invalid.Contains(character))
            {
                continue;
            }

            if (char.IsWhiteSpace(character))
            {
                if (!previousWasSpace)
                {
                    builder.Append(' ');
                    previousWasSpace = true;
                }

                continue;
            }

            builder.Append(character);
            previousWasSpace = false;
        }

        return builder.ToString().Trim();
    }

    private static string JoinWords(string value, string separator, bool lower)
    {
        string[] words = ExtractWords(value)
            .Select(word => lower ? word.ToLowerInvariant() : Capitalize(word))
            .Where(word => word.Length > 0)
            .ToArray();

        return string.Join(separator, words);
    }

    private static IEnumerable<string> ExtractWords(string value)
    {
        var current = new StringBuilder();

        foreach (char character in value)
        {
            if (char.IsLetterOrDigit(character))
            {
                current.Append(character);
                continue;
            }

            if (current.Length > 0)
            {
                yield return current.ToString();
                current.Clear();
            }
        }

        if (current.Length > 0)
        {
            yield return current.ToString();
        }
    }

    private static string Capitalize(string word)
    {
        string lower = word.ToLowerInvariant();
        return char.ToUpperInvariant(lower[0]) + lower[1..];
    }

    private static string EnforceMaxLength(string stem, int maxLength)
    {
        if (stem.Length <= maxLength)
        {
            return stem;
        }

        char separator = GetPreferredSeparator(stem);
        string[] parts = stem.Split(separator, StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length > 1)
        {
            var kept = parts.ToList();
            while (kept.Count > 1 && string.Join(separator, kept).Length > maxLength)
            {
                kept.RemoveAt(kept.Count - 1);
            }

            string shortened = string.Join(separator, kept);
            if (shortened.Length <= maxLength)
            {
                return shortened;
            }
        }

        return stem[..maxLength].TrimEnd(' ', '-', '_');
    }

    private static char GetPreferredSeparator(string stem)
    {
        if (stem.Contains(' '))
        {
            return ' ';
        }

        if (stem.Contains('_'))
        {
            return '_';
        }

        if (stem.Contains('-'))
        {
            return '-';
        }

        return ' ';
    }

    private static string RemoveExpectedExtension(string generatedName, string extension)
    {
        return generatedName.EndsWith(extension, StringComparison.OrdinalIgnoreCase)
            ? generatedName[..^extension.Length].TrimEnd()
            : generatedName;
    }
}
