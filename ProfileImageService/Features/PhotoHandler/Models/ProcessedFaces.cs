using ProfileImageService.Features.FaceApi.Models;
using System;
using System.IO;

namespace ProfileImageService.Features.PhotoHandler.Models
{
    public class ProcessedFace
    {
        public Face Face { get; set; }
        public ReadOnlyMemory<byte> PhotoOfFaceWithoutBackground { get; set; }
        public ReadOnlyMemory<byte> ProfileImage { get; set; }

        public ProcessedFace(Face face, ReadOnlyMemory<byte> photoOfFaceWithoutBackgroundStream, ReadOnlyMemory<byte> profileImageStream)
        {
            Face = face;
            PhotoOfFaceWithoutBackground = photoOfFaceWithoutBackgroundStream;
            ProfileImage = profileImageStream;
        }
    }
}
