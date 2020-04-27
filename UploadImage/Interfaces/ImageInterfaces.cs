using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UploadImage.Models;

namespace UploadImage.Interfaces
{
    public interface IImageService
    {
        Task FileCheck(FileInformation info);
        Task Save(ImageDbModel info, ImageContext context);
    }
    public interface IImageSaver
    {
        public Task Save(ImageDbModel info, ImageContext imageContext);
    }

    public interface IFileChecker
    {
        void CheckForValid(FileInformation fileInfo);
        void CheckForViruses(FileInformation fileInfo);
        void CheckForExtension(FileInformation fileInfo);
    }
}
