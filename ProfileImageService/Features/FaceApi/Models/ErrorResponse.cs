namespace ProfileImageService.Features.FaceApi.Models
{
    public class ErrorResponse
    {
        public Error Error { get; set; }

        public override string ToString()
        {
            return $"Error from Azure Face API: {Error.Code} - {Error.Message}";
        }
    }
}
