using System;
using System.IO;
using System.Threading.Tasks;
using ProfileImageService.Extensions;
using ProfileImageService.Features.FaceApi;
using ProfileImageService.Features.FaceApi.Models;
using ProfileImageService.Features.PhotoHandler.Models;
using ProfileImageService.Features.RemoveBg;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

namespace ProfileImageService.Features.PhotoHandler
{
    public class PhotoHandlerService
    {
        private const string IndiBackground = "2B3444";
        private readonly FaceApiClient _faceApiClient;
        private readonly RemoveBgClient _backgroundRemovalApiClient;

        public Func<Face, bool>? Validate { get; set; }

        public PhotoHandlerService(FaceApiClient faceApiClient, RemoveBgClient removeBgClient)
        {
            _faceApiClient = faceApiClient;
            _backgroundRemovalApiClient = removeBgClient;
        }

        public async Task<ProcessedFace[]> ProcessPhoto(MemoryStream stream)
        {
            var faces = await _faceApiClient.DedectFaces(stream);

            stream.Seek(0, SeekOrigin.Begin);

            var faceProcessingTasks = new Task<ProcessedFace>[faces.Length];

            for (var i = 0; i < faces.Length; i++)
            {
                if (Validate?.Invoke(faces[i]) ?? true)
                {
                    faceProcessingTasks[i] = ProcessFace(stream, faces[i]);
                }
            }

            return await Task.WhenAll(faceProcessingTasks);
        }

        private async Task<ProcessedFace> ProcessFace(Stream sourcePhotoStrean, Face face)
        {
            using var sourcePhoto = Image.Load(sourcePhotoStrean);

            var photoOfFace = ExtractFaceFromPhoto(sourcePhoto, face, 0.8);

            var photoOfFaceWithoutBackgroundStream = await RemoveBackgrounFromPhoto(photoOfFace);
            var photoOfFaceWithoutBackground = Image.Load(photoOfFaceWithoutBackgroundStream, new PngDecoder());

            var profileImage = CreateProfileImage(photoOfFaceWithoutBackground);

            var profileImageStream = new MemoryStream();
            profileImage.SaveAsPng(profileImageStream);

            return new ProcessedFace(face, photoOfFaceWithoutBackgroundStream, profileImageStream);
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
                // If inside the bounds tehn it's safe to crop
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

        private Task<Stream> RemoveBackgrounFromPhoto(Image photoOfFace)
        {
            using var memoryStream = new MemoryStream();
            photoOfFace.SaveAsPng(memoryStream);
            return _backgroundRemovalApiClient.RemoveBackground(memoryStream);
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
