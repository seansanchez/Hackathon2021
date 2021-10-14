using System;

namespace H21.Wellness.Models
{
    public class ScavengerHuntItemModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string[] Synonyms { get; set; }
    }
}