using System;
using ProfileImageService.Components.FaceApi.Models;

namespace ProfileImageService.Components.PhotoProcessor.Models
{
    public class ProcessedFace
    {
        public Face Face { get; set; }
        public ReadOnlyMemory<byte> PhotoOfFaceWithoutBackground { get; set; }
        public ReadOnlyMemory<byte> ProfileImage { get; set; }

        public ProcessedFace(Face face, ReadOnlyMemory<byte> photoOfFaceWithoutBackground, ReadOnlyMemory<byte> profileImage)
        {
            Face = face;
            PhotoOfFaceWithoutBackground = photoOfFaceWithoutBackground;
            ProfileImage = profileImage;
        }
    }
}
