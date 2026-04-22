if (args.Length > 0)
{
    string filePath = args[0];
    if (File.Exists(filePath))
    {
        byte[] fileBytes = File.ReadAllBytes(filePath);
        string base64String = Convert.ToBase64String(fileBytes);
        Console.WriteLine(base64String);
    }
    else
    {
        Console.WriteLine($"Error: File not found at {filePath}");
        PrintUsage();
    }
}
else
{
    PrintUsage();
}

static void PrintUsage()
{
    Console.WriteLine("Usage:");
    Console.WriteLine("  Console <file_path>");
}
