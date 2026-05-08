using Application;
using Application.Models;

namespace ApplicationTests;

public class ImageNamingPreferencesTests
{
    [Test]
    public void Resolve_WithNoConfigOrOverrides_UsesBuiltInDefaults()
    {
        var sut = new ImageNamingPreferenceResolver();

        ImageNamingPreferences result = sut.Resolve(null);

        Assert.That(result, Is.EqualTo(ImageNamingPreferences.Defaults));
    }

    [Test]
    public void Resolve_WithValidProjectConfig_OverridesDefaults()
    {
        var sut = new ImageNamingPreferenceResolver();
        var config = new ProjectLocalConfig("llava:latest", NamingConvention.Snake, 40);

        ImageNamingPreferences result = sut.Resolve(config);

        Assert.That(result.ModelName, Is.EqualTo("llava:latest"));
        Assert.That(result.NamingConvention, Is.EqualTo(NamingConvention.Snake));
        Assert.That(result.MaxNameLength, Is.EqualTo(40));
    }

    [Test]
    public void Resolve_WithRunOverrides_OverridesProjectConfig()
    {
        var sut = new ImageNamingPreferenceResolver();
        var config = new ProjectLocalConfig("llava:latest", NamingConvention.Snake, 40);
        var overrides = new RunOverrides("gemma4:e2b", NamingConvention.Kebab, 30);

        ImageNamingPreferences result = sut.Resolve(config, overrides);

        Assert.That(result.ModelName, Is.EqualTo("gemma4:e2b"));
        Assert.That(result.NamingConvention, Is.EqualTo(NamingConvention.Kebab));
        Assert.That(result.MaxNameLength, Is.EqualTo(30));
    }

    [Test]
    public void Resolve_WithEmptyModelName_Throws()
    {
        var sut = new ImageNamingPreferenceResolver();

        Assert.That(
            () => sut.Resolve(new ProjectLocalConfig(ModelName: " ")),
            Throws.TypeOf<ArgumentException>().With.Message.Contains("Model name"));
    }

    [Test]
    public void Resolve_WithTooShortMaxLength_Throws()
    {
        var sut = new ImageNamingPreferenceResolver();

        Assert.That(
            () => sut.Resolve(new ProjectLocalConfig(MaxNameLength: 2)),
            Throws.TypeOf<ArgumentException>().With.Message.Contains("Maximum name length"));
    }

    [Test]
    public void Resolve_WithInvalidProjectConfig_DoesNotAllowOverrideRescue()
    {
        var sut = new ImageNamingPreferenceResolver();
        var invalidConfig = new ProjectLocalConfig(MaxNameLength: 2);
        var validOverrides = new RunOverrides(MaxNameLength: 20);

        Assert.That(
            () => sut.Resolve(invalidConfig, validOverrides),
            Throws.TypeOf<ArgumentException>().With.Message.Contains("Maximum name length"));
    }
}
