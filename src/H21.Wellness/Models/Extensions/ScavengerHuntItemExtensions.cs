using System.Collections.Generic;
using System.Linq;
using H21.Wellness.Extensions;
using H21.Wellness.Persistence.Entities;

namespace H21.Wellness.Models.Extensions
{
    public static class ScavengerHuntItemExtensions
    {
        public static ScavengerHuntItemModel ToModel(this ScavengerHuntItemEntity source)
        {
            source.ThrowIfNull(nameof(source));

            var model = new ScavengerHuntItemModel
            {
                Id = source.Id,
                Name = source.Name,
                Description = source.Description
            };

            return model;
        }

        public static IEnumerable<ScavengerHuntItemModel> ToModels(this IEnumerable<ScavengerHuntItemEntity> source)
        {
            source.ThrowIfNull(nameof(source));

            return source.Select(x => x.ToModel());
        }
    }
}