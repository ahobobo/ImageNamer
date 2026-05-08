using Application.Models;
using System.Text.Json;

namespace ImageNamer.Cli;

public sealed class ImageRenameConfigurationLoader
{
    public ProjectLocalConfig? Load(string configPath)
    {
        if (!File.Exists(configPath))
        {
            return null;
        }

        try
        {
            string json = File.ReadAllText(configPath);
            var config = JsonSerializer.Deserialize<ImageRenameConfigurationFile>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (config is null)
            {
                throw new InvalidOperationException("Configuration file is empty.");
            }

            NamingConvention? convention = null;
            if (config.Naming is not null)
            {
                if (!ImageRenameOptionsParser.TryParseNamingConvention(config.Naming, out NamingConvention parsed))
                {
                    throw new InvalidOperationException($"Unsupported naming convention '{config.Naming}'.");
                }

                convention = parsed;
            }

            if (config.Model is not null && string.IsNullOrWhiteSpace(config.Model))
            {
                throw new InvalidOperationException("Model name must not be empty.");
            }

            return new ProjectLocalConfig(config.Model, convention, config.MaxLength);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Configuration file is malformed: {ex.Message}", ex);
        }
    }

    private sealed record ImageRenameConfigurationFile(
        string? Model,
        string? Naming,
        int? MaxLength);
}
