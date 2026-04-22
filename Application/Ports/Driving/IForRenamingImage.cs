using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.Ports.Driving
{
    public interface IForRenamingImage
    {
        Task RenameImageAsync(string imagePath);
    }
}
