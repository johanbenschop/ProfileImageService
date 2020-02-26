using ProfileImageService.Components.FaceApi.Models;
using System;

namespace ProfileImageService.Components.FaceApi
{
    public class FaceApiException : Exception
    {
        public ErrorResponse ErrorResponse { get; set; }

        public FaceApiException(ErrorResponse errorResponse) : base(errorResponse.ToString())
        {
            ErrorResponse = errorResponse;
        }
    }
}
