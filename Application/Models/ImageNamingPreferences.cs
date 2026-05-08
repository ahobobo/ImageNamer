namespace Application.Models;

public enum NamingConvention
{
    Normal,
    Snake,
    Capitalized,
    Pascal,
    Kebab
}

public sealed record ImageNamingPreferences(
    string ModelName,
    NamingConvention NamingConvention,
    int MaxNameLength)
{
    public const string DefaultModelName = "gemma4:e2b";
    public const int DefaultMaxNameLength = 20;
    public const int MinimumMaxNameLength = 3;

    public static ImageNamingPreferences Defaults { get; } = new(
        DefaultModelName,
        NamingConvention.Normal,
        DefaultMaxNameLength);
}
