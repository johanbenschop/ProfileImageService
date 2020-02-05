using System;
using System.IO;
using SixLabors.ImageSharp;

namespace ProfileImageService.Extensions
{
    public static class ImageExtensions
    {
        public static void SaveAsPng(this Image source, out Memory<byte> memory)
        {
            using var stream = new MemoryStream();
            source.SaveAsPng(stream);
            memory = new Memory<byte>(stream.GetBuffer());
        }

        public static void SaveAsJpeg(this Image source, out Memory<byte> memory)
        {
            using var stream = new MemoryStream();
            source.SaveAsJpeg(stream);
            memory = new Memory<byte>(stream.GetBuffer());
        }
    }
}
