using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UploadImage.Interfaces
{
    public interface IImageResizer
    {
        byte[] Resize(byte[] imageBytes);
        byte[] Resize(Models.FileInformation image);
    }
}
