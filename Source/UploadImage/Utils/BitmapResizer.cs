﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UploadImage.Interfaces;
using UploadImage.Models;

namespace UploadImage.Utils
{

    public class BitmapResizer : IImageResizer
    {
        private readonly int _width;
        private readonly int _height;
        private readonly ImageFormat _imageFormat;
        public BitmapResizer(int width, int height, ImageFormat imageFormat)
        {
            this._width = width;
            this._height = height;
            this._imageFormat = imageFormat;
        }


        public byte[] Resize(byte[] imageBytes)
        {
            Image currentImage = BytesToBitMap(imageBytes);
            Image resizedImage = BitMapResize(currentImage);

            return BitmapToBytes(resizedImage);
        }
        public byte[] Resize(FileInformation image)
        {
            byte[] imageBytes = image.data;

            Image currentImage = BytesToBitMap(imageBytes);
            Image resizedImage = BitMapResize(currentImage);

            return BitmapToBytes(resizedImage);
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
        private Image BytesToBitMap(byte[] bytes)
        {
            using var stream = new MemoryStream(bytes);
            return new Bitmap(stream);
        }
        private byte[] BitmapToBytes(Image image)
        {
            using MemoryStream memoryStream = new MemoryStream(1024);
            image.Save(memoryStream, this._imageFormat);

            return memoryStream.ToArray();
        }
    }
}
