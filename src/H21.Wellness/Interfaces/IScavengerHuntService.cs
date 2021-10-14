using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using H21.Wellness.Models;

namespace H21.Wellness
{
    public interface IScavengerHuntService
    {
        Task<Guid> CreateScavengerHuntAsync(
            string name,
            string description,
            uint? timeLimitInMinutes,
            IEnumerable<Guid> itemIds,
            CancellationToken cancellationToken = default);

        Task<ScavengerHuntModel> GetScavengerHuntAsync(
            Guid id,
            CancellationToken cancellationToken = default);
    }
}