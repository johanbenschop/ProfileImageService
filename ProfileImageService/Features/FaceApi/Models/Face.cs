namespace ProfileImageService.Features.FaceApi.Models
{
    public class Face
    {
        public string FaceId { get; set; }
        public FaceRectangle FaceRectangle { get; set; }
        public FaceAttributes FaceAttributes { get; set; }
    }
}