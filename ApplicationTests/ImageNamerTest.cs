using Application;
using Application.Ports;
using Application.Ports.Driving;

namespace ApplicationTests
{
    public class Tests
    {
        private ImageNamer _imageNamer;
        [SetUp]
        public void Setup()
        {
            _imageNamer = new ImageNamer();
        }

        [Test]
        public void ImageNamer_Returns_ForZero_And_Returns_0()
        {
            IForGettingZero forGettingZero = _imageNamer.GetZero();

            Assert.That(forGettingZero.GetZero(), Is.EqualTo(0));
        }
    }
}
