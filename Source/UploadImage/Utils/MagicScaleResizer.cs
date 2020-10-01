using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UploadImage.Models;
using PhotoSauce.MagicScaler;
using UploadImage.Interfaces;

namespace UploadImage.Utils
{
    public class MagicScaleResizer : IImageResizer
    {
        private readonly int _width;
        private readonly int _height;
        private readonly FileFormat fileFormat;

        public MagicScaleResizer(int width, int height, FileFormat fileFormat)
        {
            this._width = width;
            this._height = height;
            this.fileFormat = fileFormat;
        }


        public byte[] Resize(byte[] imageBytes)
        {
            using MemoryStream incomeImage = new MemoryStream(imageBytes);

            using Stream resizedImage = MagicScaleResize(incomeImage);

            StreamUtil util = new StreamUtil();

            return util.FileStreamToBytes(resizedImage);
        }
        public byte[] Resize(FileInformation image)
        {
            byte[] imageBytes = image.data;
            using MemoryStream incomeImage = new MemoryStream(imageBytes);

            using Stream resizedImage = MagicScaleResize(incomeImage);

            StreamUtil util = new StreamUtil();

            return util.FileStreamToBytes(resizedImage);
        }


        private Stream MagicScaleResize(Stream stream)
        {
            var outStream = new MemoryStream(1024);

            const int quality = 75;

            var settings = new ProcessImageSettings()
            {
                Width = this._width,

                Height = this._height,

                ResizeMode = CropScaleMode.Crop,

                SaveFormat = fileFormat,

                JpegQuality = quality,

                JpegSubsampleMode = ChromaSubsampleMode.Subsample420
            };
            MagicImageProcessor.ProcessImage(stream, outStream, settings);

            return outStream;
        }

    }
}
