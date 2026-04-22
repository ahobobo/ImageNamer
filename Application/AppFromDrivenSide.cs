using Application.Ports;
using Application.Ports.Driven;

namespace Application
{
    public class AppFromDrivenSide : IImageNamerApp
    {
        private IForInteractingWithFile _forReadingImages;

        public AppFromDrivenSide(IForInteractingWithFile forReadingImages) 
        { 
            _forReadingImages = forReadingImages;
        }
    }
}
