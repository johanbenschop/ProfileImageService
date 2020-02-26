namespace ProfileImageService.Components.FaceApi.Models
{
    public class Face
    {
        public string FaceId { get; set; } = string.Empty;
        public FaceRectangle FaceRectangle { get; set; } = new FaceRectangle();
        public FaceAttributes FaceAttributes { get; set; } = new FaceAttributes();
    }
}
