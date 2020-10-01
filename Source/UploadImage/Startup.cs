using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UploadImage.Interfaces;
using UploadImage.Models;
using UploadImage.Services;

namespace UploadImage
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            Configuration = configuration;
            WebHostEnvironment = webHostEnvironment;
        }

        public IWebHostEnvironment WebHostEnvironment { get; }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddTransient<IImageService>((service) => 
                                    new ImageService(new ImageSaver(), new ImageChecker()));

            if (WebHostEnvironment.IsDevelopment())
            {
                services.AddDbContext<ImageContext>((options) =>
                                options.UseInMemoryDatabase("ImagesDB"));
            }
            else{
                services.AddDbContext<ImageContext>((options) =>
                                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            if (WebHostEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
