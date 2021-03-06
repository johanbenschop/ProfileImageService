﻿using System.Collections.Generic;

namespace ProfileImageService.Components.FaceApi.Models
{
    public class Hair
    {
        public float Bald { get; set; }
        public bool Invisible { get; set; }
        public IEnumerable<HairColor> HairColor { get; set; } = new List<HairColor>();
    }
}
