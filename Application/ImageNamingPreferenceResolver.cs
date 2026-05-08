using Application.Models;
using Application.Ports.Driven;

namespace Application;

public sealed class ImageNamingPreferenceResolver : IForResolvingImageNamingPreferences
{
    public ImageNamingPreferences Resolve(ProjectLocalConfig? config, RunOverrides? overrides = null)
    {
        var preferences = ImageNamingPreferences.Defaults;

        if (config is not null)
        {
            preferences = preferences with
            {
                ModelName = config.ModelName ?? preferences.ModelName,
                NamingConvention = config.NamingConvention ?? preferences.NamingConvention,
                MaxNameLength = config.MaxNameLength ?? preferences.MaxNameLength
            };
            Validate(preferences);
        }

        if (overrides is not null)
        {
            preferences = preferences with
            {
                ModelName = overrides.ModelName ?? preferences.ModelName,
                NamingConvention = overrides.NamingConvention ?? preferences.NamingConvention,
                MaxNameLength = overrides.MaxNameLength ?? preferences.MaxNameLength
            };
        }

        Validate(preferences);
        return preferences;
    }

    public static void Validate(ImageNamingPreferences preferences)
    {
        if (string.IsNullOrWhiteSpace(preferences.ModelName))
        {
            throw new ArgumentException("Model name must not be empty.", nameof(preferences));
        }

        if (!Enum.IsDefined(preferences.NamingConvention))
        {
            throw new ArgumentException("Naming convention is not supported.", nameof(preferences));
        }

        if (preferences.MaxNameLength < ImageNamingPreferences.MinimumMaxNameLength)
        {
            throw new ArgumentException(
                $"Maximum name length must be at least {ImageNamingPreferences.MinimumMaxNameLength}.",
                nameof(preferences));
        }
    }
}
