using ProfileImageService.Components.FaceApi.Models;

namespace ProfileImageService.Components.PhotoProcessor
{
    public interface IPhotoValidator
    {
        bool Validate(Face face);
    }
}