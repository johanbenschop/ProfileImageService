using System;
using System.Threading.Tasks;
using ProfileImageService.Components.FaceApi;
using ProfileImageService.Extensions;
using ProfileImageService.Components.FaceApi.Models;
using ProfileImageService.Components.PhotoProcessor.Models;
using ProfileImageService.Components.RemoveBg;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using System.Collections.Generic;
using System.Linq;

namespace ProfileImageService.Components.PhotoProcessor
{
    public class PhotoProcessorService
    {
        private const string IndiBackground = "2B3444";
        private readonly FaceApiClient _faceApiClient;
        private readonly RemoveBgClient _backgroundRemovalApiClient;

        public Func<Face, bool>? Validate { get; set; }

        public PhotoProcessorService(FaceApiClient faceApiClient, RemoveBgClient removeBgClient)
        {
            _faceApiClient = faceApiClient;
            _backgroundRemovalApiClient = removeBgClient;
        }

        public async Task<IEnumerable<ProcessedFace>> ProcessPhoto(ReadOnlyMemory<byte> sourcePhoto)
        {
            var faces = (await _faceApiClient.DedectFaces(sourcePhoto)).ToArray();

            var faceProcessingTasks = new Task<ProcessedFace>[faces.Length];

            for (var i = 0; i < faces.Length; i++)
            {
                if (Validate?.Invoke(faces[i]) ?? true)
                {
                    faceProcessingTasks[i] = ProcessFace(sourcePhoto, faces[i]);
                }
            }

            return await Task.WhenAll(faceProcessingTasks);
        }

        private async Task<ProcessedFace> ProcessFace(ReadOnlyMemory<byte> sourcePhotoMemory, Face face)
        {
            using var sourcePhoto = Image.Load(sourcePhotoMemory.Span);

            var photoOfFace = ExtractFaceFromPhoto(sourcePhoto, face, 1.8);

            var photoOfFaceWithoutBackgroundMemory = await RemoveBackgrounFromPhoto(photoOfFace);
            var photoOfFaceWithoutBackground = Image.Load(photoOfFaceWithoutBackgroundMemory.Span, new PngDecoder());

            var profileImage = CreateProfileImage(photoOfFaceWithoutBackground);

            profileImage.SaveAsPng(out var profileImageMemory);

            return new ProcessedFace(face, photoOfFaceWithoutBackgroundMemory, profileImageMemory);
        }

        private static Image ExtractFaceFromPhoto(Image sourcePhoto, Face face, double inflationFactor)
        {
            var (top, left, width, height) = face.FaceRectangle;
            var faceRectangle = new Rectangle(left, top, width, height);
            faceRectangle.Inflate((int)(faceRectangle.Width * inflationFactor), (int)(faceRectangle.Width * inflationFactor));

            // Due to the inflation it is possile we're going to crop outside of the photo
            // and since this isn't possible we need to first check if we're going to do so
            if (new Rectangle(Point.Empty, sourcePhoto.Size()).Contains(faceRectangle))
            {
                // If inside the bounds then it's safe to crop
                sourcePhoto.Mutate(ctx => ctx.Crop(faceRectangle));
            }
            else
            {
                // Else we need to 'crop and pad', this copies the photo somewhere onto a new
                // canvas. This will create a white region, but that will be removed later on
                sourcePhoto = sourcePhoto.CropAndPad(faceRectangle);
            }

            return sourcePhoto;
        }

        private Task<ReadOnlyMemory<byte>> RemoveBackgrounFromPhoto(Image photoOfFace)
        {
            photoOfFace.SaveAsJpeg(out var rawProfileImage);

            return _backgroundRemovalApiClient.RemoveBackground(rawProfileImage);
        }

        private static Image<Rgba32> CreateProfileImage(Image photoOfFaceWithoutBackground)
        {
            var profileImage = new Image<Rgba32>(512, 512);

            profileImage.Mutate(ctx =>
            {
                ctx.ApplyPhoto(photoOfFaceWithoutBackground);
                ctx.ApplyFrame();
                ctx.BackgroundColor(Rgba32.FromHex(IndiBackground));
                ctx.ApplyRoundedCorners(256);
            });

            return profileImage;
        }
    }
}
