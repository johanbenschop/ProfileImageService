using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
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

namespace ProfileImageService.Components.PhotoProcessor
{
    public class PhotoProcessorService
    {
        private const string BackgroundColor = "2B3444";
        private const double InflationFactor = .8;
        private readonly FaceApiClient _faceApiClient;
        private readonly RemoveBgClient _backgroundRemovalApiClient;

        public Func<Face, bool>? Validate { get; set; } // TODO refactor this be be a service impl.

        public PhotoProcessorService(FaceApiClient faceApiClient, RemoveBgClient removeBgClient)
        {
            _faceApiClient = faceApiClient;
            _backgroundRemovalApiClient = removeBgClient;
        }

        public async Task<IEnumerable<ProcessedFace>> ProcessPhoto(ReadOnlyMemory<byte> sourcePhoto)
        {
            var faces = (await _faceApiClient.DetectFaces(sourcePhoto)).ToArray();

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
            var processedFace = new ProcessedFace(face);

            using var sourcePhoto = Image.Load(sourcePhotoMemory.Span);

            {
                var debugImage = DrawDebugImage(sourcePhoto, face, InflationFactor);
                processedFace.DebugImage = debugImage.SaveAsJpeg();
            }

            var croppedPhoto = CropFace(sourcePhoto, face, InflationFactor);
            processedFace.CroppedPhoto = croppedPhoto.SaveAsPng();

            processedFace.TransparentPhoto = await _backgroundRemovalApiClient.RemoveBackground(processedFace.CroppedPhoto);
            var transparentPhoto = Image.Load(processedFace.TransparentPhoto.Span, new PngDecoder());

            var profileImage = CreateProfileImage(transparentPhoto);
            processedFace.ProfileImage = profileImage.SaveAsPng();

            return processedFace;
        }

        private static Image CropFace(Image sourcePhoto, Face face, double inflationFactor)
        {
            var cropRectangle = face.FaceRectangle.ToRectangle();

            cropRectangle.Inflate((int)(cropRectangle.Width * inflationFactor), (int)(cropRectangle.Width * inflationFactor));

            sourcePhoto.Mutate(ctx =>
            {
                var centerPoint = new PointF(cropRectangle.Left + (cropRectangle.Width / 2), cropRectangle.Top + (cropRectangle.Height / 2));
                ctx.Rotate(-face.FaceAttributes?.HeadPose?.Roll ?? 0, centerPoint);
            });

            // Due to the inflation and rotation it is possile we're going to crop outside of the photo
            // and since this isn't allowed we need to first check if we're going to do so
            if (new Rectangle(Point.Empty, sourcePhoto.Size()).Contains(cropRectangle))
            {
                // If inside the bounds then it's safe to just crop
                sourcePhoto.Mutate(ctx =>
                {
                    ctx.Crop(cropRectangle);
                });
            }
            else
            {
                // Else we need to 'crop and pad', this creates a new image with the size of the
                // crop rectangle and places the photo at the correct location on it
                sourcePhoto = sourcePhoto.CropAndPad(cropRectangle);
            }

            return sourcePhoto;
        }

        private static Image DrawDebugImage(Image sourcePhoto, Face face, double inflationFactor)
        {
            return sourcePhoto.Clone(ctx =>
            {
                var cropRectangle = face.FaceRectangle.ToRectangle();
                ctx.Draw(Rgba32.Red, 4, cropRectangle);

                cropRectangle.Inflate((int)(cropRectangle.Width * inflationFactor), (int)(cropRectangle.Width * inflationFactor));

                ctx.Draw(Rgba32.Orange, 4, cropRectangle);

                var centerPoint = new PointF(cropRectangle.Left + (cropRectangle.Width / 2), cropRectangle.Top + (cropRectangle.Height / 2));
                ctx.Rotate(-face.FaceAttributes?.HeadPose?.Roll ?? 0, centerPoint);

                ctx.Draw(Rgba32.Green, 4, cropRectangle);
                ctx.Rotate(face.FaceAttributes?.HeadPose?.Roll ?? 0, centerPoint);
                ctx.EntropyCrop();

                ctx.BackgroundColor(Rgba32.White);
            });
        }

        private static Image<Rgba32> CreateProfileImage(Image photoOfFaceWithoutBackground)
        {
            var profileImage = new Image<Rgba32>(512, 512);

            profileImage.Mutate(ctx =>
            {
                ctx.BackgroundColor(Rgba32.FromHex(BackgroundColor));
                ctx.ApplyFrame();
                ctx.ApplyPhoto(photoOfFaceWithoutBackground);
                ctx.ApplyRoundedCorners(256);
            });

            return profileImage;
        }
    }
}
