using Application.Models;

namespace ImageNamer.Cli;

public sealed class ImageRenameOptionsParser
{
    public const string DefaultConfigFileName = "imagenamer.json";

    private static readonly IReadOnlyDictionary<string, NamingConvention> NamingConventionAliases =
        new Dictionary<string, NamingConvention>(StringComparer.OrdinalIgnoreCase)
        {
            ["normal"] = NamingConvention.Normal,
            ["snake"] = NamingConvention.Snake,
            ["snake_case"] = NamingConvention.Snake,
            ["capitalized"] = NamingConvention.Capitalized,
            ["pascal"] = NamingConvention.Pascal,
            ["kebab"] = NamingConvention.Kebab
        };

    public static string SupportedNamingConventionUsageText { get; } =
        string.Join(", ", NamingConventionAliases.Keys.Where(alias => !alias.Contains('_')));

    public ImageRenameOptionParseResult Parse(string[] args)
    {
        if (args.Length == 0)
        {
            return ImageRenameOptionParseResult.Error("Input path is required.");
        }

        if (IsHelpRequested(args[0]))
        {
            return ImageRenameOptionParseResult.Help();
        }

        var builder = new ImageRenameOptionsBuilder();

        for (int index = 0; index < args.Length; index++)
        {
            string arg = args[index];

            if (!arg.StartsWith('-'))
            {
                if (builder.InputPath is not null)
                {
                    return ImageRenameOptionParseResult.Error($"Unexpected argument '{arg}'.");
                }

                builder.InputPath = arg;
                continue;
            }

            if (IsHelpRequested(arg))
            {
                return ImageRenameOptionParseResult.Help();
            }

            if (TryApplyOption(args, ref index, builder, out string? error))
            {
                if (error is not null)
                {
                    return ImageRenameOptionParseResult.Error(error);
                }

                continue;
            }

            return ImageRenameOptionParseResult.Error(error!);
        }

        if (builder.InputPath is null)
        {
            return ImageRenameOptionParseResult.Error("Input path is required.");
        }

        return ImageRenameOptionParseResult.Success(
            new ImageRenameOptions(
                builder.InputPath,
                builder.ConfigPath,
                new RunOverrides(
                    builder.ModelName,
                    builder.NamingConvention,
                    builder.MaxNameLength)));
    }

    public static bool TryParseNamingConvention(string? value, out NamingConvention namingConvention)
    {
        if (value is not null &&
            NamingConventionAliases.TryGetValue(value.Trim(), out namingConvention))
        {
            return true;
        }

        namingConvention = default;
        return false;
    }

    private static bool IsHelpRequested(string arg)
    {
        return string.Equals(arg, "--help", StringComparison.OrdinalIgnoreCase)
            || string.Equals(arg, "-h", StringComparison.OrdinalIgnoreCase)
            || string.Equals(arg, "/?", StringComparison.OrdinalIgnoreCase);
    }

    private static bool TryReadValue(
        string[] args,
        ref int index,
        string option,
        out string? value,
        out string? error)
    {
        if (index + 1 >= args.Length)
        {
            value = null;
            error = $"{option} requires a value.";
            return false;
        }

        value = args[++index];
        error = null;
        return true;
    }

    private static bool TryApplyOption(
        string[] args,
        ref int index,
        ImageRenameOptionsBuilder builder,
        out string? error)
    {
        string option = args[index];

        if (!IsKnownOption(option))
        {
            error = $"Unknown option '{option}'.";
            return false;
        }

        if (!TryReadValue(args, ref index, option, out string? value, out error))
        {
            return true;
        }

        if (string.Equals(option, "--model", StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                error = "--model must not be empty.";
                return true;
            }

            builder.ModelName = value;
            return true;
        }

        if (string.Equals(option, "--naming", StringComparison.OrdinalIgnoreCase))
        {
            if (!TryParseNamingConvention(value, out NamingConvention parsed))
            {
                error = $"Unsupported naming convention '{value}'.";
                return true;
            }

            builder.NamingConvention = parsed;
            return true;
        }

        if (string.Equals(option, "--max-length", StringComparison.OrdinalIgnoreCase))
        {
            if (!int.TryParse(value, out int parsed))
            {
                error = "--max-length must be an integer.";
                return true;
            }

            builder.MaxNameLength = parsed;
            return true;
        }

        if (string.Equals(option, "--config", StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                error = "--config must not be empty.";
                return true;
            }

            builder.ConfigPath = Path.GetFullPath(value);
            return true;
        }
        throw new InvalidOperationException($"Option '{option}' was recognized but not handled.");
    }

    private static bool IsKnownOption(string option)
    {
        return string.Equals(option, "--model", StringComparison.OrdinalIgnoreCase)
            || string.Equals(option, "--naming", StringComparison.OrdinalIgnoreCase)
            || string.Equals(option, "--max-length", StringComparison.OrdinalIgnoreCase)
            || string.Equals(option, "--config", StringComparison.OrdinalIgnoreCase);
    }

    private sealed class ImageRenameOptionsBuilder
    {
        public string? InputPath { get; set; }

        public string ConfigPath { get; set; } =
            Path.Combine(Environment.CurrentDirectory, DefaultConfigFileName);

        public string? ModelName { get; set; }

        public NamingConvention? NamingConvention { get; set; }

        public int? MaxNameLength { get; set; }
    }
}
