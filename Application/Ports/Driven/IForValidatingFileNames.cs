namespace Application.Ports.Driven
{
    public interface IForValidatingFileNames
    {
        bool IsValidFileName(string fileName);
    }
}
