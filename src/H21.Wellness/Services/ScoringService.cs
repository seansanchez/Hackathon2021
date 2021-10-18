using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using H21.Wellness.Extensions;
using H21.Wellness.Persistence;
using H21.Wellness.Persistence.Entities;
using H21.Wellness.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace H21.Wellness.Services
{
    public class ScoringService : IScoringService
    {
        private readonly IScavengerHuntRepository _scavengerHuntRepository;
        private readonly ILogger<ScavengerHuntRepository> _logger;

        public ScoringService(
            IScavengerHuntRepository scavengerHuntRepository,
            ILogger<ScavengerHuntRepository> logger)
        {
            scavengerHuntRepository.ThrowIfNull(nameof(scavengerHuntRepository));
            logger.ThrowIfNull(nameof(logger));

            this._scavengerHuntRepository = scavengerHuntRepository;
            this._logger = logger;
        }

        /// <inheritdoc/>
        public async Task<uint> GetScore(Guid gameId, uint numItemsCompleted, uint completedInMinutes, CancellationToken cancellationToken = default)
        {
            var game = await this._scavengerHuntRepository.GetScavengerHuntAsync(gameId, cancellationToken)
                .ConfigureAwait(false);

            var totalItems = game.ItemIds.Count();
            var totalTime = game.TimeLimitInMinutes;

            var baseScoreRatio = Math.Log((numItemsCompleted / 2.0) + 1.0) / Math.Log((totalItems / 2.0) + 1.0);
            var baseScore = (uint)Math.Round(baseScoreRatio * 100.0);

            var timeBonus = (totalTime - completedInMinutes) / (totalTime / totalItems);
            var bonusScore = (uint)Math.Floor((decimal)timeBonus) * 5;

            return baseScore + bonusScore;
        }
    }
}
