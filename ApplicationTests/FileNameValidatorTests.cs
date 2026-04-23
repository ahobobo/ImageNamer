using Infrastructure.Validation;

namespace ApplicationTests;

public class FileNameValidatorTests
{
    [TestCase("Bellwether Zootopia", ExpectedResult = true)]
    [TestCase("Bellwether: Zootopia", ExpectedResult = false)]
    [TestCase("   ", ExpectedResult = false)]
    [TestCase("Bellwether_Zootopia", ExpectedResult = false)]
    public bool IsValidFileName_ReturnsExpectedResult(string fileName)
    {
        var sut = new FileNameValidator();

        return sut.IsValidFileName(fileName);
    }
}
