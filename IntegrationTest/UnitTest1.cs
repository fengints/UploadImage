using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using UploadImage;
using UploadImage.Models;
using Xunit;

namespace IntegrationTest
{
    public class BasicTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;

        public BasicTests(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/api/Image/")]
        public async Task Post_EndpointsReturnSuccess(string url)
        {
            // Arrange
            var client = _factory.CreateClient();
            var postContent = new MultipartContent();

            //Define uploading files
            using var stream = new FileStream("Data\\Desert.jpg", FileMode.Open, FileAccess.Read, FileShare.Read);
            byte[] file1 = new byte[stream.Length];
            await stream.ReadAsync(file1, 0, Convert.ToInt32(stream.Length));

            using var stream2 = new FileStream("Data\\Chrysanthemum.jpg", FileMode.Open, FileAccess.Read, FileShare.Read);
            byte[] file2 = new byte[stream2.Length];
            await stream2.ReadAsync(file2, 0, Convert.ToInt32(stream.Length));

            #region form
            MultipartFormDataContent form = new MultipartFormDataContent();

            form.Add(new ByteArrayContent(file1), "file1", "Desert.jpg");
            form.Add(new ByteArrayContent(file1), "file2", "Chrysanthemum.jpg");
            #endregion
            #region json
            JsonBase64ImageInfo[] imageInfoArray = new JsonBase64ImageInfo[2];
            imageInfoArray[0] = new JsonBase64ImageInfo() { data = file1, fileName = "Desert.jpg" };
            imageInfoArray[1] = new JsonBase64ImageInfo() { data = file2, fileName = "Chrysanthemum.jpg" };

            string jsonValue = JsonConvert.SerializeObject(imageInfoArray);

            #endregion
            #region url
            string imageUrl = "https://cdn.pixabay.com/photo/2020/04/17/14/16/landscape-5055384_960_720.jpg";

            
            var query = HttpUtility.ParseQueryString("");
            query["url"] = imageUrl;
            string imageContentUrl = url + "?" + query.ToString();

            #endregion

            // Act
            var formResponse = await client.PostAsync(url, form);
            var jsonResponse = await client.PostAsync(url, new StringContent(jsonValue, Encoding.UTF8, "application/json"));
            var urlResponse = await client.PostAsync(imageContentUrl, new StringContent(""));

            // Assert
            formResponse.EnsureSuccessStatusCode(); // Status Code 200-299
            jsonResponse.EnsureSuccessStatusCode();
            urlResponse.EnsureSuccessStatusCode();
        }
    }
    public class CustomWebApplicationFactory<TStartup>
    : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove the app's ApplicationDbContext registration.
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbContextOptions<ImageContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add ApplicationDbContext using an in-memory database for testing.
                services.AddDbContext<ImageContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });

                // Build the service provider.
                var sp = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<ImageContext>();
                    var logger = scopedServices
                        .GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                    // Ensure the database is created.
                    db.Database.EnsureCreated();
                }
            });
        }
    }

}
