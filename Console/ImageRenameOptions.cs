using Application.Models;

namespace ImageNamer.Cli;

public sealed record ImageRenameOptions(
    string InputPath,
    string ConfigPath,
    RunOverrides Overrides);

public sealed record ImageRenameOptionParseResult(
    ImageRenameOptions? Options,
    bool IsHelpRequested,
    string? ErrorMessage)
{
    public static ImageRenameOptionParseResult Help() => new(null, true, null);

    public static ImageRenameOptionParseResult Error(string message) => new(null, false, message);

    public static ImageRenameOptionParseResult Success(ImageRenameOptions options) => new(options, false, null);
}
