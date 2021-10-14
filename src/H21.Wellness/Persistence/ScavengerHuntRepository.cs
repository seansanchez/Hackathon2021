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
    public class ScavengerHuntRepository : BaseAzureBlobStorageRepository, IScavengerHuntRepository
    {
        private const string ScavengerHuntBlobContainerName = "scavengerhunts";
        private const string ScavengerHuntItemsFileName = "ScavengerHuntItems.json";

        private readonly IAzureStorageClientFactory _azureStorageClientFactory;
        private readonly ILogger<ScavengerHuntRepository> _logger;

        public ScavengerHuntRepository(
            IAzureStorageClientFactory azureStorageClientFactory,
            ILogger<ScavengerHuntRepository> logger)
            : base(azureStorageClientFactory, logger)
        {
            azureStorageClientFactory.ThrowIfNull(nameof(azureStorageClientFactory));
            logger.ThrowIfNull(nameof(logger));

            _azureStorageClientFactory = azureStorageClientFactory;
            _logger = logger;
        }

        public async Task<BlobReference> CreateScavengerHuntAsync(
            ScavengerHuntEntity entity,
            CancellationToken cancellationToken = default)
        {
            entity.ThrowIfNull(nameof(entity));

            await using var content = new MemoryStream();

            await JsonSerializer.SerializeAsync(
                    utf8Json: content,
                    value: entity,
                    options: Constants.JsonSerializerOptions,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            content.Position = 0;

            return await CreateBlobAsync(
                    blobContainerName: ScavengerHuntBlobContainerName,
                    blobName: entity.Id.ToString(),
                    content: content,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<ScavengerHuntEntity> GetScavengerHuntAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            ScavengerHuntEntity entity = null;

            var content = await OpenReadAsync(
                    blobContainerName: ScavengerHuntBlobContainerName,
                    blobName: id.ToString(),
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (content != null)
            {
                entity = await JsonSerializer.DeserializeAsync<ScavengerHuntEntity>(
                        utf8Json: content,
                        options: Constants.JsonSerializerOptions,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }

            return entity;
        }

        public async Task<Guid> GetRandomScavengerHuntIdAsync(
            CancellationToken cancellationToken = default)
        {
            var blobReference = await GetRandomBlobReferenceAsync(ScavengerHuntBlobContainerName, cancellationToken)
                .ConfigureAwait(false);

            return Guid.Parse(blobReference.BlobName);
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

            return JsonSerializer.Deserialize<IEnumerable<ScavengerHuntItemEntity>>(json, Constants.JsonSerializerOptions);
        }
    }
}