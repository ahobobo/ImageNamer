using Application.Ports.Driven;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Ports
{
    /// <summary>
    /// API for the ImageNamer App. From the Driven Side.
    /// 
    /// 
    /// </summary>
    public interface IImageNamerApp
    {
        static IImageNamerApp Create(IForInteractingWithFile forReadingImages)
        {
            return new AppFromDrivenSide(forReadingImages);
        }

    };


}
