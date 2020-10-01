using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
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
            _checker.CheckForValid(info);

            ////Check for virus before download
            //_checker.CheckForViruses();

            ////Check for extension
            _checker.CheckForExtension(info);

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
        private static readonly string[] permittedExtensions = { ".png", ".jpg", ".jpeg", ".tiff", ".tif", ".bmp", ".ico", ".gif", ".svg", ".webp" };
        private static readonly Dictionary<string, List<byte[]>> _fileSignature =
                                new Dictionary<string, List<byte[]>>
                                {
                                      { ".jpeg", new List<byte[]>
                                          {
                                              new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                                              new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                                              new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 },
                                          }
                                    },
                                      { ".jpg", new List<byte[]>
                                          {
                                              new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                                              new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
                                              new byte[] { 0xFF, 0xD8, 0xFF, 0xE8 },
                                       }
                                    },
                                    {
                                        ".png", new List<byte[]>()
                                        {
                                              new byte[] {0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }
                                        }
                                    },
                                    {
                                        ".tiff", new List<byte[]>()
                                        {
                                              new byte[] { 0x49, 0x20, 0x49 },

                                               new byte[] {  0x49, 0x49, 0x2A, 0x00 },

                                               new byte[] {  0x4D, 0x4D, 0x00, 0x2A},

                                               new byte[] {  0x4D, 0x4D, 0x00, 0x2B},
                                        }
                                    },
                                    {
                                        ".tif", new List<byte[]>()
                                        {
                                              new byte[] {0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }
                                        }
                                    },
                                    {
                                        ".bmp", new List<byte[]>()
                                        {
                                              new byte[] { 0x42, 0x4D, }
                                        }
                                    },
                                    {
                                        ".ico", new List<byte[]>()
                                        {
                                              new byte[] { 0x00, 0x00, 0x01, 0x00, }
                                        }
                                    },
                                    {
                                        ".gif", new List<byte[]>()
                                        {
                                              new byte[] { 0x47, 0x49, 0x46, 0x38,}
                                        }
                                    },
                                    {
                                        ".svg", new List<byte[]>()
                                        {
                                              new byte[] { 0x3C, }
                                        }
                                    },
                                    {
                                        ".webp", new List<byte[]>()
                                        {
                                              new byte[] { 0x52, 0x49, 0x46, 0x46 , }
                                        }
                                    },
                                };

        public void CheckForExtension(FileInformation fileInfo)
        {
            var ext = Path.GetExtension(fileInfo.fileName).ToLowerInvariant();

            //Check for extension
            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            {
                // The extension is invalid ... discontinue processing the file
                throw new ArgumentException("Wrong file extension");
            }

            //Check signature
            using Stream memStream = new MemoryStream(fileInfo.data);
            using (var reader = new BinaryReader(memStream))
            {
                var signatures = _fileSignature[ext];
                var headerBytes = reader.ReadBytes(signatures.Max(m => m.Length));

                if (signatures.Any(signature =>
                    headerBytes.Take(signature.Length).SequenceEqual(signature)) == false)
                {
                    throw new ArgumentException("Wrong file extension");
                }
            }
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
