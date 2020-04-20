using System.Security.Cryptography;
using ProfileImageService.Components.PhotoProcessor;
using ProfileImageService.Extensions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ProfileImageService.Designs
{
    public class LiveCodersDesign : IDesign
    {
        public Image<Rgba32> ApplyDesign(Image image)
        {
            var profileImage = new Image<Rgba32>(512, 512);

            profileImage.Mutate(ctx =>
            {
                image.Mutate(ctx =>
                {
                    ctx.Vignette(new GraphicsOptions(true, PixelColorBlendingMode.Darken, 0.5f), Color.Black);
                });

                ctx.DrawFrame($"assets/livecoders/background-{RandomNumberGenerator.GetInt32(4)}.jpg");
                ctx.Vignette(new GraphicsOptions(true, PixelColorBlendingMode.Darken, 0.5f), Color.Purple);
                ctx.DrawPhoto(image);
            });

            return profileImage;
        }
    }
}
