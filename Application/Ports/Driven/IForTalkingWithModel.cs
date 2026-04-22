using Application.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.Ports.Driven
{
    public interface IForTalkingWithModel
    {
        Task<ImageFile> GetNewImageNameAsync(ImageFile originalImageFile);
    }
}
