using System;
using System.Threading;
using System.Threading.Tasks;

namespace H21.Wellness.Services.Interfaces
{
    public interface IScoringService
    {
        /// <summary>
        ///     Returns a score for a completed game
        /// </summary>
        /// <param name="gameId">The ID of the scavenger hunt.</param>
        /// <param name="numItemsCompleted">The number of items completed.</param>
        /// <param name="completedInMinutes">The time it took to complete.</param>
        /// <returns></returns>
        Task<uint> GetScore(Guid gameId, uint numItemsCompleted, uint completedInMinutes, CancellationToken cancellationToken);
    }
}
