using Application.Ports.Driving;
using Infrastructure.ForReadingImages;

namespace ImageNamer.Cli;

public sealed record ImageRenameRunResult(int SuccessfulCount, int FailureCount);

public sealed class ImageRenameRunner
{
    private readonly IForRenamingImage _renamer;

    public ImageRenameRunner(IForRenamingImage renamer)
    {
        _renamer = renamer;
    }

    public async Task<ImageRenameRunResult> RunAsync(
        string inputPath,
        TextWriter? output = null,
        TextWriter? error = null)
    {
        output ??= Console.Out;
        error ??= Console.Error;

        if (File.Exists(inputPath))
        {
            await _renamer.RenameImageAsync(inputPath);
            output.WriteLine($"Image renamed successfully: {inputPath}");
            return new ImageRenameRunResult(1, 0);
        }

        if (Directory.Exists(inputPath))
        {
            return await RenameDirectoryAsync(inputPath, output, error);
        }

        throw new FileNotFoundException("Input path not found.", inputPath);
    }

    private async Task<ImageRenameRunResult> RenameDirectoryAsync(
        string directoryPath,
        TextWriter output,
        TextWriter error)
    {
        var enumerationOptions = new EnumerationOptions
        {
            RecurseSubdirectories = true,
            IgnoreInaccessible = true,
            ReturnSpecialDirectories = false
        };

        List<string> imagePaths = Directory
            .EnumerateFiles(directoryPath, "*", enumerationOptions)
            .Where(ImageFileExtensions.IsSupportedImagePath)
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (imagePaths.Count == 0)
        {
            output.WriteLine("No supported images were found.");
            return new ImageRenameRunResult(0, 0);
        }

        int successfulCount = 0;
        int failureCount = 0;

        foreach (string imagePath in imagePaths)
        {
            try
            {
                await _renamer.RenameImageAsync(imagePath);
                successfulCount++;
                output.WriteLine($"Image renamed successfully: {imagePath}");
            }
            catch (Exception ex)
            {
                failureCount++;
                error.WriteLine($"Error renaming {imagePath}: {ex.Message}");
            }
        }

        return new ImageRenameRunResult(successfulCount, failureCount);
    }
}
