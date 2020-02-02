using ProfileImageService.Features.FaceApi.Models;

namespace ProfileImageService.Features.PhotoHandler.Models
{
    public class ProgressEvent
    {
        public EventType EventType { get; set; }

        public Face[] Faces { get; set; }

        public ProgressEvent(EventType eventType)
        {
            EventType = eventType;
        }
    }

    public enum EventType
    {
        FacesReceived
    }
}
