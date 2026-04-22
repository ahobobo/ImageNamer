using Application;
using Application.Ports;
using Application.Ports.Driving;

if (args.Length > 0 && args[0].ToLower() == "getzero")
{
    IImageNamerApp app = new ImageNamer();
    IForGettingZero getter = app.GetZero();
    int result = getter.GetZero();
    Console.WriteLine(result);
}
else
{
    Console.WriteLine("Usage: Console getzero");
}
