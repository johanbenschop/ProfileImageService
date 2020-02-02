namespace ProfileImageService.Features.FaceApi.Models
{
    public class FaceRectangle
    {
        public int Top { get; set; }
        public int Left { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public void Deconstruct(out int top, out int left, out int width, out int height)
        {
            top = Top;
            left = Left;
            width = Width;
            height = Height;
        }
    }
}