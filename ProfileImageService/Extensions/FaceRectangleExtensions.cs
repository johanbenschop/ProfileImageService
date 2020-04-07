using ProfileImageService.Components.FaceApi.Models;
using SixLabors.Primitives;

namespace ProfileImageService.Extensions
{
    public static class FaceRectangleExtensions
    {
        public static Rectangle ToRectangle(this FaceRectangle faceRectangle)
        {
            var (top, left, width, height) = faceRectangle;
            return new Rectangle(left, top, width, height);
        } 
    }
}
