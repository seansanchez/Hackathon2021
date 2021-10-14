using System;
using H21.Wellness.Services.Interfaces;

namespace H21.Wellness.Services
{
    public class ScoringService : IScoringService
    {
        /// <inheritdoc/>
        public uint GetScore(Guid gameId, uint numItemsCompleted, uint completedInMinutes)
        {
           // var game = // TODO

           var totalItems = 10;
           var totalTime = 20;

           var baseScoreRatio = Math.Log((numItemsCompleted / 2.0) + 1.0) / Math.Log((totalItems / 2.0) + 1.0);
           var baseScore = (uint) Math.Round(baseScoreRatio * 100.0);

           var timeBonus = (totalTime - completedInMinutes) / (totalTime / totalItems);
           var bonusScore = (uint)Math.Floor((decimal)timeBonus) * 5;

           return baseScore + bonusScore;
        }
    }
}
