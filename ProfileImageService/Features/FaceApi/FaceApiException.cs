using ProfileImageService.Features.FaceApi.Models;
using System;

namespace ProfileImageService.Features.FaceApi
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