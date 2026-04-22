using Application;
using Application.Models;
using Application.Ports.Driven;
using Application.Ports.Driving;

namespace ApplicationTests;

public class ImageRenamerTests
{
    [Test]
    public async Task RenameImageAsync_UsesModelResultToRenameFile()
    {
        var originalFile = new ImageFile(
            "original.webp",
            ".webp",
            @"C:\images\original.webp",
            Convert.ToBase64String(new byte[] { 1, 2, 3, 4 }));

        var fileStore = new RecordingFileStore(originalFile);
        var model = new RecordingModel(originalFile with { Name = "renamed.webp" });
        IForRenamingImage sut = new ImageRenamer(fileStore, model);

        await sut.RenameImageAsync(@"C:\images\original.webp");

        Assert.That(fileStore.ReadPath, Is.EqualTo(@"C:\images\original.webp"));
        Assert.That(model.ReceivedImage, Is.EqualTo(originalFile));
        Assert.That(fileStore.OriginalFile, Is.EqualTo(originalFile));
        Assert.That(fileStore.RenamedFile, Is.EqualTo(originalFile with { Name = "renamed.webp" }));
    }

    private sealed class RecordingFileStore : IForInteractingWithFile
    {
        private readonly ImageFile _file;

        public RecordingFileStore(ImageFile file)
        {
            _file = file;
        }

        public string? ReadPath { get; private set; }

        public ImageFile ReadFile(string path)
        {
            ReadPath = path;
            return _file;
        }

        public void RenameFile(ImageFile originalFile, ImageFile renamedFile)
        {
            OriginalFile = originalFile;
            RenamedFile = renamedFile;
        }

        public ImageFile? OriginalFile { get; private set; }

        public ImageFile? RenamedFile { get; private set; }
    }

    private sealed class RecordingModel : IForTalkingWithModel
    {
        private readonly ImageFile _result;

        public RecordingModel(ImageFile result)
        {
            _result = result;
        }

        public ImageFile? ReceivedImage { get; private set; }

        public Task<ImageFile> GetNewImageNameAsync(ImageFile originalImageFile)
        {
            ReceivedImage = originalImageFile;
            return Task.FromResult(_result);
        }
    }
}
