using Application.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Ports.Driven
{
    public interface IForInteractingWithFile
    {
        ImageFile ReadFile(string path);
        string RenameFile(ImageFile originalFile, ImageFile renamedFile);
    }
}
