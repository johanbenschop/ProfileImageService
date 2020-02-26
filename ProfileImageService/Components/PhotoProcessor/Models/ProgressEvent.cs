using System.Collections.Generic;
using System.Linq;
using ProfileImageService.Components.FaceApi.Models;

namespace ProfileImageService.Components.PhotoProcessor.Models
{
    public class ProgressEvent
    {
        public EventType EventType { get; set; }

        public IEnumerable<Face> Faces { get; set; }

        public ProgressEvent(EventType eventType)
        {
            EventType = eventType;
            Faces = Enumerable.Empty<Face>();
        }
    }

    public enum EventType
    {
        FacesReceived
    }
}
