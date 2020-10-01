using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UploadImage.Models
{
    public class ImageContext: DbContext
    {
        public ImageContext(DbContextOptions<ImageContext> options): base(options)
        {

        }

        public DbSet<ImageDbModel> Images { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }
    }
    public class ImageDbModel
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public byte[] ImageData { get; set; }
        public byte[] PreviewData { get; set; }
        public DateTime AddedDate { get; set; }
    }
}
