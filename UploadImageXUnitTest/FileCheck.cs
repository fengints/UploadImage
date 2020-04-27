using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UploadImage.Models;
using UploadImage.Services;
using UploadImage.Utils;
using Xunit;

namespace UploadImageXUnitTest
{
    public class FileCheck
    {
        [Fact]
        public void CheckValidExtension()
        {
            var dir = "Data";
            var files = Directory.EnumerateFiles(dir);

            ImageChecker imageChecker = new ImageChecker();
            StreamUtil utils = new StreamUtil();

            foreach (var file in files)
            {
                string fileName = file.Substring(dir.Length + 1);
                using var stream = new FileStream(file, FileMode.Open, FileAccess.Read);

                FileInformation fileInformation = new FileInformation() {data = utils.FileStreamToBytes(stream), fileName = fileName };

                //assert
                imageChecker.CheckForExtension(fileInformation);
            }

        }
    }
}
