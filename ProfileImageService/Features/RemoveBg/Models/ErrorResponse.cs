namespace ProfileImageService.Features.RemoveBg.Models
{
    public class ErrorResponse
    {
        public Error[] Errors { get; set; }

        public override string ToString()
        {
            return $"{Errors.Length} errors, first: {Errors[0].Title}: {Errors[0].Detail}";
        }
    }
}
