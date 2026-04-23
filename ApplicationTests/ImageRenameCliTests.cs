using ImageNamer.Cli;

namespace ApplicationTests;

[NonParallelizable]
public class ImageRenameCliTests
{
    [Test]
    public async Task RunAsync_WithHelpFlag_PrintsUsageAndExitsSuccessfully()
    {
        TextWriter originalOut = Console.Out;
        int originalExitCode = Environment.ExitCode;
        var output = new StringWriter();

        try
        {
            Console.SetOut(output);

            await ImageRenameCli.RunAsync(["--help"]);

            Assert.That(Environment.ExitCode, Is.EqualTo(0));
            Assert.That(output.ToString(), Does.Contain("Usage:"));
            Assert.That(output.ToString(), Does.Contain("<directory_path>"));
            Assert.That(output.ToString(), Does.Contain("--help"));
        }
        finally
        {
            Environment.ExitCode = originalExitCode;
            Console.SetOut(originalOut);
        }
    }
}
