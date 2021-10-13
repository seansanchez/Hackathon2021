using System;
using System.Collections.Generic;

namespace H21.Wellness.Models
{
    public class ScavengerHuntModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public uint TimeLimitInMinutes { get; set; } = 20;

        public IEnumerable<ScavengerHuntItemModel> Items { get; set; }
    }
}