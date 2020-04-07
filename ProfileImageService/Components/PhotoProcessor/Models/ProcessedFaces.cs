using System;
using ProfileImageService.Components.FaceApi.Models;

namespace ProfileImageService.Components.PhotoProcessor.Models
{
    public class ProcessedFace
    {
        public Face Face { get; }
        public ReadOnlyMemory<byte> CroppedPhoto { get; internal set; }
        public ReadOnlyMemory<byte> TransparentPhoto { get; internal set; }
        public ReadOnlyMemory<byte> ProfileImage { get; internal set; }
        public ReadOnlyMemory<byte> DebugImage { get; internal set; }

        public ProcessedFace(Face face, ReadOnlyMemory<byte> croppedPhoto, ReadOnlyMemory<byte> photoOfFaceWithoutBackground, ReadOnlyMemory<byte> profileImage)
        {
            Face = face;
            CroppedPhoto = croppedPhoto;
            TransparentPhoto = photoOfFaceWithoutBackground;
            ProfileImage = profileImage;
        }

        public ProcessedFace(Face face)
        {
            Face = face;
        }
    }
}
