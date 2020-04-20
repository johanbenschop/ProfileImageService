using ProfileImageService.Components.PhotoProcessor;
using ProfileImageService.Extensions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ProfileImageService.Designs
{
    public class SentiaDesign : IDesign
    {
        public Image<Rgba32> ApplyDesign(Image image)
        {
            var profileImage = new Image<Rgba32>(512, 512);

            profileImage.Mutate(ctx =>
            {
                ctx.DrawFrame("assets/sentia/frame-background.png");
                ctx.DrawPhoto(image);
                //ctx.DrawFrame("assets/sentia/frame-foreground.png");
            });

            return profileImage;
        }
    }
}
