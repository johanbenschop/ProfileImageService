using System.Collections.Generic;
using System.Linq;

namespace ProfileImageService.Components.RemoveBg.Models
{
    public class ErrorResponse
    {
        public IEnumerable<Error> Errors { get; set; } = Enumerable.Empty<Error>();

        public override string ToString()
        {
            return $"{Errors.Count()} errors: {string.Join(", ", Errors.Select(x => $"{x.Title} - {x.Detail}"))}";
        }
    }
}
