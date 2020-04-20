using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ProfileImageService.Components.PhotoProcessor
{
    public interface IDesign
    {
        Image<Rgba32> ApplyDesign(Image image);
    }
}
