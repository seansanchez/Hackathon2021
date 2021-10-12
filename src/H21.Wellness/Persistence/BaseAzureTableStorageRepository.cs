using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using H21.Wellness.Extensions;
using H21.Wellness.Persistence.Interfaces;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;

namespace H21.Wellness.Persistence
{
    public abstract class BaseAzureTableStorageRepository
    {
        private readonly IAzureStorageClientFactory _azureStorageClientFactory;
        private readonly ILogger<BaseAzureTableStorageRepository> _logger;

        protected BaseAzureTableStorageRepository(
            IAzureStorageClientFactory azureStorageClientFactory,
            ILogger<BaseAzureTableStorageRepository> logger)
        {
            azureStorageClientFactory.ThrowIfNull(nameof(azureStorageClientFactory));
            logger.ThrowIfNull(nameof(logger));

            _azureStorageClientFactory = azureStorageClientFactory;
            _logger = logger;
        }

        protected async Task CreateTableEntityAsync<T>(
            string tableName,
            T entity,
            CancellationToken cancellationToken = default)
            where T : TableEntity, new()
        {
            tableName.ThrowIfNullOrWhitespace(nameof(tableName));
            entity.ThrowIfNull(nameof(entity));

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                var cloudTable = await this._azureStorageClientFactory.GetCloudTableAsync(tableName, cancellationToken);
                var operation = TableOperation.Insert(entity);

                await cloudTable.ExecuteAsync(operation, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                stopwatch.Stop();
                this._logger.LogDebug($"{nameof(CreateTableEntityAsync)} took {stopwatch.ElapsedMilliseconds} milliseconds to complete.");
            }
        }
    }
}