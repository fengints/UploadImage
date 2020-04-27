using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UploadImage.Interfaces;
using UploadImage.Models;
using UploadImage.Utils;

namespace UploadImage.Services
{
    public class ImageService : IImageService
    {
        private readonly IImageSaver _saver;
        private readonly IFileChecker _checker;
        
        public ImageService(IImageSaver saver, IFileChecker checker)
        {
            this._saver = saver;
            this._checker = checker;
        }
        public async Task Save(ImageDbModel model, ImageContext dbContext)
        {
            await _saver.Save(model, dbContext);
        }


        public Task FileCheck(FileInformation info)
        {
            //Check for valid
            //_checker.CheckForValid();

            ////Check for virus before upload
            //_checker.CheckForViruses();

            ////Check for extension
            //_checker.CheckForExtension();

            return Task.FromResult<object>(null);
        }

    }


    public class ImageSaver : IImageSaver
    {
        public async Task Save(ImageDbModel imageModel, ImageContext context)
        {
            await context.Images.AddAsync(imageModel);
            await context.SaveChangesAsync();
        }
    }

    public class ImageChecker : IFileChecker
    {
        public void CheckForExtension(FileInformation fileInfo)
        {
            throw new NotImplementedException();
        }

        public void CheckForValid(FileInformation fileInfo)
        {
            if (fileInfo.data == Array.Empty<byte>())
                throw new NullReferenceException("Array is empty");
        }

        public void CheckForViruses(FileInformation fileInfo)
        {
            throw new NotImplementedException();
        }
    }
}
