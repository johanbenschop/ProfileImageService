using ProfileImageService.Features.FaceApi.Models;
using System.IO;

namespace ProfileImageService.Features.PhotoHandler.Models
{
    public class ProcessedFace
    {
        public Face Face { get; set; }
        public Stream PhotoOfFaceWithoutBackgroundStream { get; set; }
        public Stream ProfileImageStream { get; set; }

        public ProcessedFace(Face face, Stream photoOfFaceWithoutBackgroundStream, Stream profileImageStream)
        {
            Face = face;
            PhotoOfFaceWithoutBackgroundStream = photoOfFaceWithoutBackgroundStream;
            ProfileImageStream = profileImageStream;

            PhotoOfFaceWithoutBackgroundStream.Seek(0, SeekOrigin.Begin);
            ProfileImageStream.Seek(0, SeekOrigin.Begin);
        }
    }
}
