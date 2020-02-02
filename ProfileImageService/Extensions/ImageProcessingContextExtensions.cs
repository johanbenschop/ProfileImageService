using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using SixLabors.Shapes;

namespace ProfileImageService.Extensions
{
    public static class ImageProcessingContextExtensions
    {
        public static void ApplyPhoto(this IImageProcessingContext ctx, Image image)
        {
            using var overlayImage = image;
            var result = overlayImage.Clone(
                ctx2 => ctx2.Resize(new ResizeOptions { Mode = ResizeMode.Crop, Size = new Size(512) }));

            ctx.DrawImage(result, PixelColorBlendingMode.Normal, 1);
        }

        public static void ApplyFrame(this IImageProcessingContext ctx)
        {
            using var overlayImage = Image.Load("assets/frame.png");
            ctx.DrawImage(overlayImage, PixelColorBlendingMode.Normal, 1);
        }

        public static void ApplyRoundedCorners(this IImageProcessingContext ctx, float cornerRadius)
        {
            var (width, height) = ctx.GetCurrentSize();
            var corners = BuildCorners(width, height, cornerRadius);

            var graphicOptions = new GraphicsOptions(true)
            {
                AlphaCompositionMode = PixelAlphaCompositionMode.DestOut // Enforces that any part of this shape that has color is punched out of the background
            };

            // Mutating in here as we already have a cloned original
            // use any color (not Transparent), so the corners will be clipped
            ctx.Fill(graphicOptions, Rgba32.LimeGreen, corners);
        }

        private static IPathCollection BuildCorners(int imageWidth, int imageHeight, float cornerRadius)
        {
            // First create a square
            var rect = new RectangularPolygon(-0.5f, -0.5f, cornerRadius, cornerRadius);

            // Then cut out of the square a circle so we are left with a corner
            var cornerTopLeft = rect.Clip(new EllipsePolygon(cornerRadius - 0.5f, cornerRadius - 0.5f, cornerRadius));

            // Corner is now a corner shape positions top left
            // lets make 3 more positioned correctly, we can do that by translating the original around the center of the image
            var rightPos = imageWidth - cornerTopLeft.Bounds.Width + 1;
            var bottomPos = imageHeight - cornerTopLeft.Bounds.Height + 1;

            // Move it across the width of the image - the width of the shape
            var cornerTopRight = cornerTopLeft.RotateDegree(90).Translate(rightPos, 0);
            var cornerBottomLeft = cornerTopLeft.RotateDegree(-90).Translate(0, bottomPos);
            var cornerBottomRight = cornerTopLeft.RotateDegree(180).Translate(rightPos, bottomPos);

            return new PathCollection(cornerTopLeft, cornerBottomLeft, cornerTopRight, cornerBottomRight);
        }

        public static Image<Rgba32> CropAndPad(this Image image, Rectangle crop)
        {
            var croppedImage = new Image<Rgba32>(Configuration.Default, crop.Width, crop.Height, Color.Azure);
            //var croppedImage = new Image<Rgba32>(crop.Width, crop.Height);

            croppedImage.Mutate(ctx => ctx.DrawImage(image, new Point(crop.X * -1, crop.Y * -1), 1));

            return croppedImage;
        }
    }
}
