using H21.Wellness.Extensions;
using H21.Wellness.Persistence.Interfaces;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

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
                _logger.LogDebug($"{nameof(CreateTableEntityAsync)} took {stopwatch.ElapsedMilliseconds} milliseconds to complete.");
            }
        }

        protected async Task UpdateTableEntityAsync<T>(
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
                var operation = TableOperation.Replace(entity);

                var result = await cloudTable.ExecuteAsync(operation, cancellationToken).ConfigureAwait(false);

                entity = (T)result.Result;
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogDebug($"{nameof(UpdateTableEntityAsync)} took {stopwatch.ElapsedMilliseconds} milliseconds to complete.");
            }
        }

        protected async Task<T> GetTableEntityAsync<T>(
            string tableName,
            string partitionKey,
            string rowKey,
            CancellationToken cancellationToken = default)
            where T : TableEntity, new()
        {
            tableName.ThrowIfNullOrWhitespace(nameof(tableName));
            partitionKey.ThrowIfNullOrWhitespace(nameof(partitionKey));
            rowKey.ThrowIfNullOrWhitespace(nameof(rowKey));

            T entity;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                var cloudTable = await _azureStorageClientFactory.GetCloudTableAsync(tableName, cancellationToken).ConfigureAwait(false);
                var operation = TableOperation.Retrieve<T>(partitionKey, rowKey);

                var result = await cloudTable.ExecuteAsync(operation, cancellationToken).ConfigureAwait(false);

                entity = (T)result.Result;
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogDebug($"{nameof(GetTableEntityAsync)} took {stopwatch.ElapsedMilliseconds} milliseconds to complete.");
            }

            return entity;
        }

        protected async Task<IEnumerable<T>> GetTableEntitiesAsync<T>(
            string tableName,
            string filter,
            CancellationToken cancellationToken)
            where T : TableEntity, new()
        {
            tableName.ThrowIfNullOrWhitespace(nameof(tableName));
            filter.ThrowIfNullOrWhitespace(nameof(filter));

            var entities = new List<T>();

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                var query = new TableQuery<T>
                {
                    FilterString = filter
                };

                var cloudTable = await _azureStorageClientFactory.GetCloudTableAsync(tableName, cancellationToken).ConfigureAwait(false);
                TableContinuationToken continuationToken = null;

                do
                {
                    var querySegment = await cloudTable
                        .ExecuteQuerySegmentedAsync<T>(
                            query: query,
                            token: continuationToken,
                            cancellationToken: cancellationToken)
                        .ConfigureAwait(false);

                    continuationToken = querySegment.ContinuationToken;

                    entities.AddRange(querySegment.Results);
                }
                while (continuationToken != null && !cancellationToken.IsCancellationRequested);
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogDebug($"{nameof(GetTableEntitiesAsync)} took {stopwatch.ElapsedMilliseconds} milliseconds to complete.");
            }

            return entities;
        }
    }
}