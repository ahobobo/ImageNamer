using Application.Models;

namespace ImageNamer.Cli;

public sealed class ImageRenameOptionsParser
{
    public const string DefaultConfigFileName = "imagenamer.json";

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

        string? inputPath = null;
        string configPath = Path.Combine(Environment.CurrentDirectory, DefaultConfigFileName);
        string? modelName = null;
        NamingConvention? namingConvention = null;
        int? maxNameLength = null;

        for (int index = 0; index < args.Length; index++)
        {
            string arg = args[index];

            if (!arg.StartsWith('-'))
            {
                if (inputPath is not null)
                {
                    return ImageRenameOptionParseResult.Error($"Unexpected argument '{arg}'.");
                }

                inputPath = arg;
                continue;
            }

            if (IsHelpRequested(arg))
            {
                return ImageRenameOptionParseResult.Help();
            }

            if (string.Equals(arg, "--model", StringComparison.OrdinalIgnoreCase))
            {
                if (!TryReadValue(args, ref index, arg, out modelName, out string? error))
                {
                    return ImageRenameOptionParseResult.Error(error!);
                }

                if (string.IsNullOrWhiteSpace(modelName))
                {
                    return ImageRenameOptionParseResult.Error("--model must not be empty.");
                }

                continue;
            }

            if (string.Equals(arg, "--naming", StringComparison.OrdinalIgnoreCase))
            {
                if (!TryReadValue(args, ref index, arg, out string? value, out string? error))
                {
                    return ImageRenameOptionParseResult.Error(error!);
                }

                if (!TryParseNamingConvention(value, out NamingConvention parsed))
                {
                    return ImageRenameOptionParseResult.Error($"Unsupported naming convention '{value}'.");
                }

                namingConvention = parsed;
                continue;
            }

            if (string.Equals(arg, "--max-length", StringComparison.OrdinalIgnoreCase))
            {
                if (!TryReadValue(args, ref index, arg, out string? value, out string? error))
                {
                    return ImageRenameOptionParseResult.Error(error!);
                }

                if (!int.TryParse(value, out int parsed))
                {
                    return ImageRenameOptionParseResult.Error("--max-length must be an integer.");
                }

                maxNameLength = parsed;
                continue;
            }

            if (string.Equals(arg, "--config", StringComparison.OrdinalIgnoreCase))
            {
                if (!TryReadValue(args, ref index, arg, out string? value, out string? error))
                {
                    return ImageRenameOptionParseResult.Error(error!);
                }

                if (string.IsNullOrWhiteSpace(value))
                {
                    return ImageRenameOptionParseResult.Error("--config must not be empty.");
                }

                configPath = Path.GetFullPath(value);
                continue;
            }

            return ImageRenameOptionParseResult.Error($"Unknown option '{arg}'.");
        }

        if (inputPath is null)
        {
            return ImageRenameOptionParseResult.Error("Input path is required.");
        }

        return ImageRenameOptionParseResult.Success(
            new ImageRenameOptions(
                inputPath,
                configPath,
                new RunOverrides(modelName, namingConvention, maxNameLength)));
    }

    public static bool TryParseNamingConvention(string? value, out NamingConvention namingConvention)
    {
        switch (value?.Trim().ToLowerInvariant())
        {
            case "normal":
                namingConvention = NamingConvention.Normal;
                return true;
            case "snake":
            case "snake_case":
                namingConvention = NamingConvention.Snake;
                return true;
            case "capitalized":
                namingConvention = NamingConvention.Capitalized;
                return true;
            case "pascal":
                namingConvention = NamingConvention.Pascal;
                return true;
            case "kebab":
                namingConvention = NamingConvention.Kebab;
                return true;
            default:
                namingConvention = default;
                return false;
        }
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
}
