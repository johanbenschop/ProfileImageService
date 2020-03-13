using System.Collections.Generic;

namespace ProfileImageService.Components.FaceApi.Models
{
    public class FaceAttributes
    {
        public float Smile { get; set; }
        public HeadPose? HeadPose { get; set; }
        public string Gender { get; set; } = string.Empty;
        public float Age { get; set; }
        public FacialHair? FacialHair { get; set; }
        public string Glasses { get; set; } = string.Empty;
        public Emotion? Emotion { get; set; }
        public Blur? Blur { get; set; }
        public Exposure? Exposure { get; set; }
        public Noise? Noise { get; set; }
        public Makeup? Makeup { get; set; }
        public IEnumerable<Accessory> Accessories { get; set; } = new List<Accessory>();
        public Occlusion? Occlusion { get; set; }
        public Hair? Hair { get; set; }
    }
}
