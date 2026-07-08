using Application.Models;
using ImageNamer.Cli;
using TestsShared;

namespace ApplicationTests;

public class ImageRenameOptionsTests
{
    [Test]
    public void Parse_WithInputOnly_UsesDefaultConfigPath()
    {
        using var temp = new TemporaryWorkingDirectory();
        var sut = new ImageRenameOptionsParser();

        ImageRenameOptionParseResult result = sut.Parse(["image.webp"]);

        Assert.That(result.ErrorMessage, Is.Null);
        Assert.That(result.Options!.InputPath, Is.EqualTo("image.webp"));
        Assert.That(
            Path.GetFileName(result.Options.ConfigPath),
            Is.EqualTo(ImageRenameOptionsParser.BundledDefaultConfigFileName));
    }

    [Test]
    public void Parse_WithProjectLocalConfig_UsesCurrentWorkingDirectoryConfig()
    {
        using var temp = new TemporaryWorkingDirectory();
        temp.WriteConfig("""{ "naming": "snake" }""");
        var sut = new ImageRenameOptionsParser();

        ImageRenameOptionParseResult result = sut.Parse(["image.webp"]);

        Assert.That(result.ErrorMessage, Is.Null);
        Assert.That(result.Options!.ConfigPath, Is.EqualTo(temp.ConfigPath));
    }

    [Test]
    public void Parse_WithAllOverrides_ReturnsRunOverrides()
    {
        var sut = new ImageRenameOptionsParser();

        ImageRenameOptionParseResult result = sut.Parse([
            "image.webp",
            "--model", "llava:latest",
            "--naming", "kebab",
            "--max-length", "30",
            "--config", "custom.json"
        ]);

        Assert.That(result.ErrorMessage, Is.Null);
        Assert.That(result.Options!.Overrides.ModelName, Is.EqualTo("llava:latest"));
        Assert.That(result.Options.Overrides.NamingConvention, Is.EqualTo(NamingConvention.Kebab));
        Assert.That(result.Options.Overrides.MaxNameLength, Is.EqualTo(30));
        Assert.That(result.Options.ConfigPath, Is.EqualTo(Path.GetFullPath("custom.json")));
    }

    [TestCase("--unknown", "Unknown option")]
    [TestCase("--naming", "--naming requires a value")]
    [TestCase("--max-length abc", "--max-length must be an integer")]
    [TestCase("--naming unsupported", "Unsupported naming convention")]
    public void Parse_WithInvalidInput_ReturnsError(string argsText, string expectedMessage)
    {
        var sut = new ImageRenameOptionsParser();
        string[] args = ["image.webp", .. argsText.Split(' ')];

        ImageRenameOptionParseResult result = sut.Parse(args);

        Assert.That(result.ErrorMessage, Does.Contain(expectedMessage));
    }
}
