using ProfileImageService.Components.PhotoProcessor;
using ProfileImageService.Extensions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ProfileImageService.Designs
{
    public class IndivirtualDesign : IDesign
    {
        private const string BackgroundColor = "2B3444";

        public Image<Rgba32> ApplyDesign(Image image)
        {
            var profileImage = new Image<Rgba32>(512, 512);

            profileImage.Mutate(ctx =>
            {
                ctx.BackgroundColor(Rgba32.FromHex(BackgroundColor));
                ctx.DrawFrame("assets/indivirtual/frame-background.png");
                ctx.DrawPhoto(image);
                ctx.DrawFrame("assets/indivirtual/frame-foreground.png");
                ctx.ApplyRoundedCorners(256);
            });

            return profileImage;
        }
    }
}
