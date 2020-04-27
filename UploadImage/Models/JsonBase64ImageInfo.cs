using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UploadImage.Utils;

namespace UploadImage.Models
{
    public class JsonBase64ImageInfo: FileInformation
    {
        [Required]
        [JsonConverter(typeof(JsonBase64Formatter))]
        public override byte[] data { get; set; }
    }
}
