using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using H21.Wellness.Extensions;
using H21.Wellness.Models;
using H21.Wellness.Models.Extensions;
using H21.Wellness.Persistence;
using H21.Wellness.Persistence.Entities;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;

namespace H21.Wellness
{
    public class ScavengerHuntService : IScavengerHuntService
    {
        private readonly IScavengerHuntRepository _scavengerHuntRepository;
        private readonly ILogger<ScavengerHuntService> _logger;

        public ScavengerHuntService(
            IScavengerHuntRepository scavengerHuntRepository,
            ILogger<ScavengerHuntService> logger)
        {
            scavengerHuntRepository.ThrowIfNull(nameof(scavengerHuntRepository));
            logger.ThrowIfNull(nameof(logger));

            _scavengerHuntRepository = scavengerHuntRepository;
            _logger = logger;
        }

        public async Task<Guid> CreateScavengerHuntAsync(
            string name,
            string description,
            uint? timeLimitInMinutes,
            IEnumerable<Guid> itemIds,
            CancellationToken cancellationToken = default)
        {
            name.ThrowIfNullOrWhiteSpace(nameof(name));
            description.ThrowIfNullOrWhiteSpace(nameof(description));
            itemIds.ThrowIfNull(nameof(itemIds));

            var scavengerHuntItems =
                await _scavengerHuntRepository
                    .GetScavengerHuntItemsAsync(cancellationToken)
                    .ConfigureAwait(false);

            var validItemIds = scavengerHuntItems.Select(x => x.Id).Intersect(itemIds);

            var entity = new ScavengerHuntEntity
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description,
                TimeLimitInMinutes = timeLimitInMinutes,
                ItemIds = validItemIds
            };

            var blobReference = await _scavengerHuntRepository.CreateScavengerHuntAsync(entity, cancellationToken).ConfigureAwait(false);

            return Guid.Parse(blobReference.BlobName);
        }

        public async Task<ScavengerHuntModel> GetScavengerHuntAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            ScavengerHuntModel model = null;

            var scavengerHuntEntity =
                await _scavengerHuntRepository
                    .GetScavengerHuntAsync(id, cancellationToken)
                    .ConfigureAwait(false);

            if (scavengerHuntEntity != null)
            {
                var items = new List<ScavengerHuntItemModel>();

                if (scavengerHuntEntity.ItemIds.Any())
                {
                    foreach (var itemId in scavengerHuntEntity.ItemIds)
                    {
                        var scavengerHuntItemEntity = await _scavengerHuntRepository.GetScavengerHuntItemAsync(itemId, cancellationToken).ConfigureAwait(false);

                        if (scavengerHuntItemEntity != null)
                        {
                            items.Add(scavengerHuntItemEntity.ToModel());
                        }
                    }
                }

                model = new ScavengerHuntModel
                {
                    Id = scavengerHuntEntity.Id,
                    Name = scavengerHuntEntity.Name,
                    Description = scavengerHuntEntity.Description,
                    TimeLimitInMinutes = scavengerHuntEntity.TimeLimitInMinutes,
                    Items = items
                };
            }

            return model;
        }

        public async Task<ScavengerHuntModel> GetRandomScavengerHuntAsync(CancellationToken cancellationToken = default)
        {
            var id = await _scavengerHuntRepository.GetRandomScavengerHuntIdAsync(cancellationToken)
                .ConfigureAwait(false);

            return await GetScavengerHuntAsync(id, cancellationToken).ConfigureAwait(false);
        }
    }
}