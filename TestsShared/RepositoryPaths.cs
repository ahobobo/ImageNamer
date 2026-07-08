namespace TestsShared;

public static class RepositoryPaths
{
    public static string Root { get; } = ResolveRoot();

    public static string TestDataDirectory => Path.Combine(Root, "TestData");

    private static string ResolveRoot()
    {
        string current = AppContext.BaseDirectory;

        while (!File.Exists(Path.Combine(current, "ImageNamer.slnx")))
        {
            string? parent = Path.GetDirectoryName(current);
            if (parent is null || parent == current)
            {
                throw new DirectoryNotFoundException(
                    $"Could not locate ImageNamer.slnx from {AppContext.BaseDirectory}.");
            }

            current = parent;
        }

        return current;
    }
}
