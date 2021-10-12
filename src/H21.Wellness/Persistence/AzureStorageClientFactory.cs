using System;
using System.Threading;
using System.Threading.Tasks;
using H21.Wellness.Extensions;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace H21.Wellness.Persistence
{
    public class AzureStorageClientFactory
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IOptions<AzureStorageOptions> _options;
        private readonly SemaphoreSlim _semaphore;

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
            _semaphore = new SemaphoreSlim(1);
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
                    await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

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
                    _semaphore.Release();
                }
            }

            return cloudTable;
        }
    }
}