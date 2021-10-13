using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using H21.Wellness.Persistence.Entities;

namespace H21.Wellness.Persistence
{
    public interface IScavengerHuntRepository
    {
        Task<ScavengerHuntItemEntity> GetScavengerHuntItemAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<ScavengerHuntItemEntity>> GetScavengerHuntItemsAsync(
            CancellationToken cancellationToken = default);
    }
}