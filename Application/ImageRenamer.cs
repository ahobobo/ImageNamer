using Application.Ports.Driven;
using Application.Ports.Driving;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application
{
    public class ImageRenamer : IForRenamingImage
    {
        //driven ports
        private readonly IForTalkingWithModel _model;
        private readonly IForInteractingWithFile _imgScr;

        public ImageRenamer(IForInteractingWithFile forReadingImages, IForTalkingWithModel forTalkingWithModel)
        {
            _imgScr = forReadingImages;
            _model = forTalkingWithModel;
        }
        public async Task RenameImageAsync(string imagePath)
        {
            var imageFile = _imgScr.ReadFile(imagePath);

            var renamedImageFile = await _model.GetNewImageNameAsync(imageFile);

            _imgScr.RenameFile(imageFile, renamedImageFile);
        }
    }
}
