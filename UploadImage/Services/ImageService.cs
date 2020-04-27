using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UploadImage.CInterface;
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


        public Task SecurityCheck(ImageInfo info)
        {
            ////Check for virus before upload
            //_checker.CheckForViruses();

            ////Check for extension
            //_checker.CheckForExtension();


            return Task.FromResult<object>(null);
        }

    }

    //Probably will be deleted
    public class ImageSaver : IImageSaver
    {
        public async Task Save(ImageDbModel imageModel, ImageContext context)
        {
            await context.Images.AddAsync(imageModel);
            await context.SaveChangesAsync();
        }
    }

    public interface IFileChecker
    {
        void CheckForViruses();
        void CheckForExtension();
    }
    public class ImageChecker : IFileChecker
    {
        public void CheckForExtension()
        {
            throw new NotImplementedException();
        }

        public void CheckForViruses()
        {
            throw new NotImplementedException();
        }
    }
}
