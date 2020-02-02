namespace ProfileImageService.Features.FaceApi.Models
{
    public class ErrorResponse
    {
        public string Code { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return $"Error from Azure Face API: {Code} - {Message}";
        }
    }
}