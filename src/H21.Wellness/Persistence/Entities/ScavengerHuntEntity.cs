using System;
using System.Collections.Generic;

namespace H21.Wellness.Persistence.Entities
{
    public class ScavengerHuntEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public uint? TimeLimitInMinutes { get; set; } = 20;

        public IEnumerable<Guid> ItemIds { get; set; }
    }
}