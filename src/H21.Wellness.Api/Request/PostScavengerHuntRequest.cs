using System;
using System.Collections.Generic;

namespace H21.Wellness.Api.Request
{
    public class PostScavengerHuntRequest
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public uint? TimeLimitInMinutes { get; set; }

        public IEnumerable<Guid> ItemIds { get; set; }
    }
}