using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using H21.Wellness.Extensions;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace H21.Wellness.Persistence
{
    public class AzureStorageClientFactory : IAzureStorageClientFactory
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IOptions<AzureStorageOptions> _options;
        private readonly SemaphoreSlim _cloudTableSemaphore;
        private readonly SemaphoreSlim _blobContainerClientSemaphore;

        public AzureStorageClientFactory(
            IMemoryCache memoryCache,
            IOptions<AzureStorageOptions> options)
        {
            memoryCache.ThrowIfNull(nameof(memoryCache));
            options.ThrowIfNull(nameof(options));
            options.Value.ThrowIfNull($"{nameof(options)}.{nameof(options.Value)}");
            options.Value.ConnectionString.ThrowIfNullOrWhitespace($"{nameof(options)}.{nameof(options.Value)}.{nameof(options.Value.ConnectionString)}");

            _memoryCache = memoryCache;
            _options = options;
            _cloudTableSemaphore = new SemaphoreSlim(1);
            _blobContainerClientSemaphore = new SemaphoreSlim(1);
        }

        public async Task<CloudTable> GetCloudTableAsync(
            string tableName,
            CancellationToken cancellationToken = default)
        {
            tableName.ThrowIfNullOrWhitespace(nameof(tableName));

            var cacheKey = $"{nameof(BaseAzureTableStorageRepository)}:CloudTable:{tableName}";

            var exists = _memoryCache.TryGetValue(cacheKey, out CloudTable cloudTable);

            if (!exists)
            {
                try
                {
                    await _cloudTableSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

                    exists = _memoryCache.TryGetValue(cacheKey, out cloudTable);

                    if (!exists)
                    {
                        var cloudStorageAccount = CloudStorageAccount.Parse(_options.Value.ConnectionString);
                        var cloudTableClient = cloudStorageAccount.CreateCloudTableClient();

                        cloudTable = cloudTableClient.GetTableReference(tableName);
                        await cloudTable.CreateIfNotExistsAsync(cancellationToken).ConfigureAwait(false);

                        var cacheExpiration = DateTimeOffset.UtcNow.AddMilliseconds(_options.Value.StorageClientCacheExpirationInMs);

                        _memoryCache.Set(cacheKey, cloudTable, cacheExpiration);
                    }
                }
                finally
                {
                    _cloudTableSemaphore.Release();
                }
            }

            return cloudTable;
        }

        public async Task<BlobContainerClient> GetBlobContainerClientAsync(
            string blobContainerName,
            CancellationToken cancellationToken = default)
        {
            blobContainerName.ThrowIfNullOrWhitespace(nameof(blobContainerName));

            var cacheKey = $"{nameof(BaseAzureTableStorageRepository)}:BlobContainer:{blobContainerName}";

            var exists = _memoryCache.TryGetValue(cacheKey, out BlobContainerClient blobContainerClient);

            if (!exists)
            {
                try
                {
                    await _blobContainerClientSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

                    exists = _memoryCache.TryGetValue(cacheKey, out blobContainerClient);

                    if (!exists)
                    {
                        var blobServiceClient = new BlobServiceClient(_options.Value.ConnectionString);

                        blobContainerClient = blobServiceClient.GetBlobContainerClient(blobContainerName);
                        await blobContainerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

                        var cacheExpiration = DateTimeOffset.UtcNow.AddMilliseconds(_options.Value.StorageClientCacheExpirationInMs);

                        _memoryCache.Set(cacheKey, blobContainerClient, cacheExpiration);
                    }
                }
                finally
                {
                    _blobContainerClientSemaphore.Release();
                }
            }

            return blobContainerClient;
        }

        public async Task<BlobClient> GetBlobClientAsync(
            string blobContainerName,
            string blobName,
            CancellationToken cancellationToken = default)
        {
            blobContainerName.ThrowIfNullOrWhitespace(nameof(blobContainerName));
            blobName.ThrowIfNullOrWhitespace(nameof(blobName));

            var blobContainerClient =
                await GetBlobContainerClientAsync(
                        blobContainerName: blobContainerName,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

            return blobContainerClient.GetBlobClient(blobName);
        }
    }
}