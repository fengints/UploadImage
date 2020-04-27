using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UploadImage.Models;

namespace UploadImage.CInterface
{
    public interface IImageService
    {
        Task SecurityCheck(ImageInfo info);
        Task Save(ImageDbModel info, ImageContext context);
    }
    public interface IImageSaver
    {
        public Task Save(ImageDbModel info, ImageContext imageContext);
    }
}
