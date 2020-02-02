using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ProfileImageService.Features.FaceApi;
using ProfileImageService.Features.PhotoHandler;
using ProfileImageService.Features.RemoveBg;
using ProfileImageService.Settings;

namespace ProfileImageService.ConsoleApp
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .AddUserSecrets("8A5141D1-33BC-4B1E-8051-F57FB87FC5C5")
               .Build();

            var settings = new ProfileImageServiceSettings();
            configuration.Bind(settings);
            var removeBgClient = new RemoveBgClient(new HttpClient(), settings);
            var faceApiClient = new FaceApiClient(new HttpClient(), settings);
            var photoHandlerService = new PhotoHandlerService(faceApiClient, removeBgClient);

            photoHandlerService.Validate = face =>
            {
                Console.WriteLine($"Validating face '{face.FaceId}'...");
                return true;
            };

            var photoStream = new MemoryStream(File.ReadAllBytes("assets/adult-1868750_1280.jpg"));
            var processedFaces = await photoHandlerService.ProcessPhoto(photoStream);

            foreach (var processedFace in processedFaces)
            {
                using var rawFile = File.Create("output/raw.png");
                await processedFace.PhotoOfFaceWithoutBackgroundStream.CopyToAsync(rawFile);

                using var profileImageFile = File.Create("output/profileImage.png");
                await processedFace.ProfileImageStream.CopyToAsync(profileImageFile);
            }
        }
    }
}
