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

namespace UploadImage.Utils
{
    public class ImageResizer : IImageResizer
    {
        private readonly int _width;
        private readonly int _height;
        private readonly ImageFormat _imageFormat;
        public ImageResizer(int width, int height, ImageFormat imageFormat)
        {
            this._width = width;
            this._height = height;
            this._imageFormat = imageFormat;
        }


        public byte[] Resize(byte[] imageBytes)
        {
            using MemoryStream incomeImage = new MemoryStream(imageBytes);

            using Stream resizedImage = MagicScaleResize(incomeImage);

            StreamUtil util = new StreamUtil();

            return util.FileStreamToBytes(resizedImage);
        }
        public byte[] Resize(Models.FileInformation image)
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

                SaveFormat = FileFormat.Jpeg,

                JpegQuality = quality,

                JpegSubsampleMode = ChromaSubsampleMode.Subsample420
            };
            MagicImageProcessor.ProcessImage(stream, outStream, settings);

            return outStream;
        }
        private Image BitMapResize(Image image)
        {
            var destRect = new Rectangle(0, 0, this._width, this._height);
            var destImage = new Bitmap(this._width, this._height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
        private Image BytesToImage(byte[] bytes)
        {
            using var stream = new MemoryStream(bytes);
            return new Bitmap(stream);
        }
        private byte[] ImageToBytes(Image image)
        {
            using MemoryStream memoryStream = new MemoryStream(1024);
            image.Save(memoryStream, this._imageFormat);

            return memoryStream.ToArray();
        }

    }
    public interface IImageResizer
    {
        byte[] Resize(byte[] imageBytes);
        byte[] Resize(Models.FileInformation image);
    }
}
