using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace UploadImage.Utils
{
    public class StreamUtil
    {
        public byte[] FileStreamToBytes(FileStream stream)
        {
            byte[] jpgBytes = new byte[stream.Length];
            stream.Read(jpgBytes, 0, Convert.ToInt32(stream.Length));
            return jpgBytes;
        }
    }
}
