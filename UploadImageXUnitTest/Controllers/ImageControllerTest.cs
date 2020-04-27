using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using UploadImage.Controllers;
using UploadImage.Interfaces;
using UploadImage.Models;
using UploadImage.Services;
using Xunit;

namespace UploadImageXUnitTest.Controllers
{
    public class ImageControllerTest
    {
        private readonly Mock<ILogger<ImageController>> _mockLogger;
        private readonly Mock<IImageService> _mockService;
        private readonly ImageController _controller;
        private readonly ImageContext _mockDbContext;

        //IImageService if you have separated test for ImageService Class
        public ImageControllerTest()
        {
            _mockLogger = new Mock<ILogger<ImageController>>();
            _mockService = new Mock<IImageService>();
            _mockDbContext =  InitializeDatabase();
            _controller = new ImageController(_mockLogger.Object, _mockService.Object, _mockDbContext);
        }
        private ImageContext InitializeDatabase()
        {
            //Create database context
            var options = new DbContextOptionsBuilder<ImageContext>()
                                        .UseInMemoryDatabase(databaseName: "TestImageDatabase")
                                        .Options;
            return new ImageContext(options);
        }


        [Fact]
        public void ValidTestPostJson()
        {
            //Arrange
            JsonBase64ImageInfo[] imageInfoArray = new JsonBase64ImageInfo[2];

            using var stream = new FileStream("Data\\Desert.jpg", FileMode.Open, FileAccess.Read, FileShare.Read);
            byte[] array = new byte[stream.Length];
            stream.Read(array, 0, Convert.ToInt32(stream.Length));

            using var stream2 = new FileStream("Data\\Chrysanthemum.jpg", FileMode.Open, FileAccess.Read, FileShare.Read);
            byte[] array2 = new byte[stream.Length];
            stream.Read(array, 0, Convert.ToInt32(stream.Length));

            imageInfoArray[0] = new JsonBase64ImageInfo() { data = array, fileName = "Desert.jpg" };
            imageInfoArray[1] = new JsonBase64ImageInfo() { data = array, fileName = "Chrysanthemum.jpg" };

            //Act
            var result = _controller.PostJson(imageInfoArray);

            //Assert
            Assert.IsType<OkResult>(result.Result);
        }
        [Fact]
        public void ValidTestPostForm()
        {
            //Arrange
            FormFileCollection formFiles = new FormFileCollection();

            using var stream = new FileStream("Data\\Desert.jpg", FileMode.Open, FileAccess.Read, FileShare.Read);
            formFiles.Add(new FormFile(stream, 0, stream.Length, "image1", "Desert.jpg"));

            using var stream2 = new FileStream("Data\\Chrysanthemum.jpg", FileMode.Open, FileAccess.Read, FileShare.Read);
            formFiles.Add(new FormFile(stream, 0, stream.Length, "image2", "Chrysanthemum.jpg"));

            IFormCollection collection 
                = new FormCollection(new System.Collections.Generic.Dictionary<string, Microsoft.Extensions.Primitives.StringValues>(),
                                    files: formFiles);

            //Act
            var result = _controller.PostForm(collection);
            //Assert
            Assert.IsType<OkResult>(result.Result);
        }
        [Fact]
        public void ValidTestPostFromUrl()
        {
            //Arrange
            string imageUrl = "https://cdn.pixabay.com/photo/2020/04/17/14/16/landscape-5055384_960_720.jpg";

            //Act
            var result = _controller.PostFromUrl(imageUrl, "new file");

            //Assert
            Assert.IsType<OkResult>(result.Result);
        }



        [Fact]
        public void InvalidTestPostJson()
        {
            //Arrange
            JsonBase64ImageInfo[] imageInfoArray = new JsonBase64ImageInfo[1];
            JsonBase64ImageInfo[] imageInfoArray2 = new JsonBase64ImageInfo[1];

            imageInfoArray2[0] = new JsonBase64ImageInfo() {fileName = "Desert" };

            //Act
            var result = _controller.PostJson(imageInfoArray);
            var result2 = _controller.PostJson(imageInfoArray2);

            //Assert
            Assert.IsNotType<OkResult>(result.Result);
            Assert.IsNotType<OkResult>(result2.Result);
        }
        [Fact]
        public void InvalidTestPostForm()
        {
            //Arrange
            FormFileCollection formFiles = new FormFileCollection();

            IFormCollection collection
                = new FormCollection(new System.Collections.Generic.Dictionary<string, Microsoft.Extensions.Primitives.StringValues>(),
                                    files: formFiles);

            //Act
            var result = _controller.PostForm(null);
            var result1 = _controller.PostForm(collection);

            //Assert
            Assert.IsNotType<OkResult>(result.Result);
            Assert.IsNotType<OkResult>(result1.Result);

        }
        [Fact]
        public void InvalidTestPostFromUrl()
        {
            //Arrange
            string invalidUrl = "https://google.com";

            //Act
            var result = _controller.PostFromUrl(invalidUrl, "new file");

            //Assert
            Assert.IsNotType<OkResult>(result.Result);
            //Assert.ThrowsAny<AggregateException>(()=> _controller.PostFromUrl(null).Result);
            //Assert.ThrowsAny<AggregateException>(() => _controller.PostFromUrl("").Result);
            Assert.ThrowsAny<AggregateException>(() => _controller.PostFromUrl("Not url", "new file").Result);
        }
    }
}
