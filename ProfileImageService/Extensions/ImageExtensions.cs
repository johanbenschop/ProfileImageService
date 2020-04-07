using System;
using System.IO;
using SixLabors.ImageSharp;

namespace ProfileImageService.Extensions
{
    public static class ImageExtensions
    {
        public static Memory<byte> SaveAsPng(this Image source)
        {
            using var stream = new MemoryStream();
            source.SaveAsPng(stream);
            return new Memory<byte>(stream.GetBuffer());
        }

        public static Memory<byte> SaveAsJpeg(this Image source)
        {
            using var stream = new MemoryStream();
            source.SaveAsJpeg(stream);
            return new Memory<byte>(stream.GetBuffer());
        }
    }
}
