using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using H21.Wellness.Extensions;
using H21.Wellness.Persistence.Entities;
using Microsoft.Extensions.Logging;

namespace H21.Wellness.Persistence
{
    public class ScavengerHuntRepository : IScavengerHuntRepository
    {
        private const string ScavengerHuntItemsFileName = "ScavengerHuntItems.json";

        private readonly ILogger<ScavengerHuntRepository> _logger;

        public ScavengerHuntRepository(ILogger<ScavengerHuntRepository> logger)
        {
            logger.ThrowIfNull(nameof(logger));

            _logger = logger;
        }

        public async Task<ScavengerHuntItemEntity> GetScavengerHuntItemAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var scavengerHuntItems = await GetScavengerHuntItemsAsync(cancellationToken).ConfigureAwait(false);

            return scavengerHuntItems.FirstOrDefault(x => x.Id == id);
        }

        public async Task<IEnumerable<ScavengerHuntItemEntity>> GetScavengerHuntItemsAsync(
            CancellationToken cancellationToken = default)
        {
            var filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), ScavengerHuntItemsFileName);

            var json = await File.ReadAllBytesAsync(filePath, cancellationToken).ConfigureAwait(false);

            return JsonSerializer.Deserialize<IEnumerable<ScavengerHuntItemEntity>>(json, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }
}