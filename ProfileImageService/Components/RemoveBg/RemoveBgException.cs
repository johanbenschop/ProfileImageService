using System;
using ProfileImageService.Components.RemoveBg.Models;

namespace ProfileImageService.Components.RemoveBg
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
