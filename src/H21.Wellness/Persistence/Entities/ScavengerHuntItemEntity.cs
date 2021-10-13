using System;
using System.Collections.Generic;

namespace H21.Wellness.Persistence.Entities
{
    public class ScavengerHuntItemEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public IEnumerable<string> Synonyms { get; set; }
    }
}