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
           var maxItemCountScore = 100;

           var baseScore = (uint)Math.Round(Math.Log(numItemsCompleted / 160.0) / Math.Log(maxItemCountScore / 160.0));

           var timeBonus = (totalTime - completedInMinutes) / (totalTime / totalItems);
           var bonusScore = (uint)Math.Floor((decimal)timeBonus) * 5;

           return baseScore + bonusScore;
        }
    }
}
