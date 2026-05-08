namespace Application.Models;

public sealed record ProjectLocalConfig(
    string? ModelName = null,
    NamingConvention? NamingConvention = null,
    int? MaxNameLength = null);

public sealed record RunOverrides(
    string? ModelName = null,
    NamingConvention? NamingConvention = null,
    int? MaxNameLength = null);

public enum PreferenceSource
{
    BuiltInDefault,
    ProjectLocalConfig,
    RunOverride
}
