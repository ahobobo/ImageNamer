using Application.Models;
using Application.Ports.Driven;
using ImageNamer.Cli;

namespace TestsShared;

public static class CliDriver
{
    public static async Task<(int ExitCode, string Output, string Error)> RunAsync(string[] args)
    {
        var output = new StringWriter();
        var error = new StringWriter();
        TextWriter originalOutput = Console.Out;
        TextWriter originalError = Console.Error;
        int originalExitCode = Environment.ExitCode;

        try
        {
            Environment.ExitCode = 0;
            Console.SetOut(output);
            Console.SetError(error);

            await ImageRenameCli.RunAsync(args, new ImageRenameCliDependencies
            {
                ModelFactory = static _ => new DeterministicModel()
            });

            return (Environment.ExitCode, output.ToString(), error.ToString());
        }
        finally
        {
            Console.SetOut(originalOutput);
            Console.SetError(originalError);
            Environment.ExitCode = originalExitCode;
        }
    }

    private sealed class DeterministicModel : IForTalkingWithModel
    {
        public Task<ImageFile> GetNewImageNameAsync(ImageFile originalImageFile)
        {
            string extension = originalImageFile.Extension.StartsWith('.')
                ? originalImageFile.Extension
                : $".{originalImageFile.Extension}";

            return Task.FromResult(originalImageFile with
            {
                Name = $"deterministic descriptive image name{extension}"
            });
        }
    }
}
