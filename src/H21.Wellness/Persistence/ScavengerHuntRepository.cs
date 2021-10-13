using H21.Wellness.Extensions;
using Microsoft.Extensions.Logging;

namespace H21.Wellness.Persistence
{
    public class ScavengerHuntRepository : IScavengerHuntRepository
    {
        private const string ScavengerHuntItemsFile = "ScavengerHuntItems.json";

        private readonly ILogger<ScavengerHuntRepository> _logger;

        public ScavengerHuntRepository(ILogger<ScavengerHuntRepository> logger)
        {
            logger.ThrowIfNull(nameof(logger));

            _logger = logger;
        }
    }
}