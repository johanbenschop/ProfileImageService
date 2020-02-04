using System;
using ProfileImageService.Features.RemoveBg.Models;

namespace ProfileImageService.Features.RemoveBg
{
    public class RemoveBgException : Exception
    {
        public ErrorResponse ErrorResponse { get; set; }

        public RemoveBgException(ErrorResponse errorResponse) : base(errorResponse.ToString())
        {
            ErrorResponse = errorResponse;
        }
    }
}
