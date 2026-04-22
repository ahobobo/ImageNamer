using Application.Ports;
using Application.Ports.Driving;

namespace Application
{
    public class ImageNamer : IImageNamerApp
    {
        private IForGettingZero _forGettingZero;

        public ImageNamer() 
        { 
            _forGettingZero = new ForGettingZero();
        }

        public IForGettingZero GetZero()
        {
            return _forGettingZero;
        }
    }
}
