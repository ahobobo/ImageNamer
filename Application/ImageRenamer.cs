using Application.Models;
using Application.Ports.Driven;
using Application.Ports.Driving;

namespace Application;

public class ImageRenamer : IForRenamingImage
{
    private readonly IForInteractingWithFile _fileOperator;
    private readonly IForTalkingWithModel _model;
    private readonly IForFormattingImageName _nameFormatter;
    private readonly ImageNamingPreferences _preferences;

    public ImageRenamer(IForInteractingWithFile forReadingImages, IForTalkingWithModel forTalkingWithModel)
        : this(forReadingImages, forTalkingWithModel, new ImageNameFormatter(), ImageNamingPreferences.Defaults)
    {
    }

    public ImageRenamer(
        IForInteractingWithFile forReadingImages,
        IForTalkingWithModel forTalkingWithModel,
        IForFormattingImageName nameFormatter,
        ImageNamingPreferences preferences)
    {
        _fileOperator = forReadingImages;
        _model = forTalkingWithModel;
        _nameFormatter = nameFormatter;
        _preferences = preferences;
    }

    public async Task<string> RenameImageAsync(string imagePath)
    {
        var imageFile = _fileOperator.ReadFile(imagePath);
        var renamedImageFile = await _model.GetNewImageNameAsync(imageFile);

        string formattedName = _nameFormatter.FormatName(
            renamedImageFile.Name,
            imageFile.Extension,
            _preferences);

        return _fileOperator.RenameFile(
            imageFile,
            renamedImageFile with { Name = formattedName, Extension = imageFile.Extension });
    }
}
