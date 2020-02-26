namespace ProfileImageService.Components.FaceApi.Models
{
    public class Error
    {
        public int? StatusCode { get; set; }
        public string? Code { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
