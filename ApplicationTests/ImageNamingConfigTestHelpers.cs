namespace ApplicationTests;

internal sealed class TemporaryWorkingDirectory : IDisposable
{
    private readonly string _originalDirectory;

    public TemporaryWorkingDirectory()
    {
        _originalDirectory = Environment.CurrentDirectory;
        Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "ImageNamerTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path);
        Environment.CurrentDirectory = Path;
    }

    public string Path { get; }

    public string ConfigPath => System.IO.Path.Combine(Path, "imagenamer.json");

    public void WriteConfig(string json)
    {
        File.WriteAllText(ConfigPath, json);
    }

    public void Dispose()
    {
        Environment.CurrentDirectory = _originalDirectory;
        if (Directory.Exists(Path))
        {
            Directory.Delete(Path, true);
        }
    }
}
