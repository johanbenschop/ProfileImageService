namespace ProfileImageService.Components.FaceApi.Models
{
    public class ErrorResponse
    {
        public Error Error { get; set; } = new Error();

        public override string ToString()
        {
            return $"Error from Azure Face API: {Error.StatusCode}{Error.Code} - {Error.Message}";
        }
    }
}
