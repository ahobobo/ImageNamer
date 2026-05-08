using Application;
using Application.Models;

namespace ApplicationTests;

public class ImageNameFormatterTests
{
    [Test]
    public void FormatName_WithNormal_PreservesValidPunctuationAndCasing()
    {
        var sut = new ImageNameFormatter();
        var preferences = ImageNamingPreferences.Defaults with { NamingConvention = NamingConvention.Normal, MaxNameLength = 50 };

        string result = sut.FormatName("Red Sunset - Beach Photo!.webp", ".webp", preferences);

        Assert.That(result, Is.EqualTo("Red Sunset - Beach Photo!.webp"));
    }

    [Test]
    public void FormatName_WithNormal_RemovesInvalidCharacters()
    {
        var sut = new ImageNameFormatter();
        var preferences = ImageNamingPreferences.Defaults with { NamingConvention = NamingConvention.Normal, MaxNameLength = 50 };

        string result = sut.FormatName("Red<Sunset>:Beach", ".webp", preferences);

        Assert.That(result, Does.Not.Contain("<"));
        Assert.That(result, Does.Not.Contain(">"));
        Assert.That(result, Does.Not.Contain(":"));
        Assert.That(result, Does.EndWith(".webp"));
    }

    [TestCase(NamingConvention.Snake, "red_sunset_beach_photo.webp")]
    [TestCase(NamingConvention.Capitalized, "Red Sunset Beach Photo.webp")]
    [TestCase(NamingConvention.Pascal, "RedSunsetBeachPhoto.webp")]
    [TestCase(NamingConvention.Kebab, "red-sunset-beach-photo.webp")]
    public void FormatName_WithConvention_FormatsExpectedStem(NamingConvention convention, string expected)
    {
        var sut = new ImageNameFormatter();
        var preferences = ImageNamingPreferences.Defaults with { NamingConvention = convention, MaxNameLength = 50 };

        string result = sut.FormatName("red sunset beach photo", ".webp", preferences);

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void FormatName_WhenOverMaxLength_RemovesTrailingWords()
    {
        var sut = new ImageNameFormatter();
        var preferences = ImageNamingPreferences.Defaults with { NamingConvention = NamingConvention.Kebab, MaxNameLength = 10 };

        string result = sut.FormatName("red sunset beach photo", ".webp", preferences);

        Assert.That(result, Is.EqualTo("red-sunset.webp"));
    }

    [Test]
    public void FormatName_WithSingleOverlongWord_HardTruncates()
    {
        var sut = new ImageNameFormatter();
        var preferences = ImageNamingPreferences.Defaults with { NamingConvention = NamingConvention.Normal, MaxNameLength = 5 };

        string result = sut.FormatName("extraordinary", ".webp", preferences);

        Assert.That(result, Is.EqualTo("extra.webp"));
    }

    [Test]
    public void FormatName_WithTooShortMaxLength_Throws()
    {
        var sut = new ImageNameFormatter();
        var preferences = ImageNamingPreferences.Defaults with { MaxNameLength = 2 };

        Assert.That(
            () => sut.FormatName("red sunset", ".webp", preferences),
            Throws.TypeOf<ArgumentException>().With.Message.Contains("Maximum name length"));
    }
}
