using System;
using System.Threading;
using System.Threading.Tasks;
using H21.Wellness.Extensions;
using H21.Wellness.Persistence.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace H21.Wellness.Persistence
{
    public class AzureTableStorageHealthCheck : IHealthCheck
    {
        private const string HealthCheckTableName = "healthcheck";

        private readonly IAzureStorageClientFactory _azureStorageClientFactory;
        private readonly ILogger<AzureTableStorageHealthCheck> _logger;

        public AzureTableStorageHealthCheck(
            IAzureStorageClientFactory azureStorageClientFactory,
            ILogger<AzureTableStorageHealthCheck> logger)
        {
            azureStorageClientFactory.ThrowIfNull(nameof(azureStorageClientFactory));
            logger.ThrowIfNull(nameof(logger));

            _azureStorageClientFactory = azureStorageClientFactory;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = new CancellationToken())
        {
            context.ThrowIfNull(nameof(context));

            HealthCheckResult healthCheckResult;

            try
            {
                var cloudTable = await _azureStorageClientFactory
                    .GetCloudTableAsync(HealthCheckTableName, cancellationToken)
                    .ConfigureAwait(false);

                await cloudTable.CreateIfNotExistsAsync(cancellationToken).ConfigureAwait(false);

                if (await cloudTable.ExistsAsync(cancellationToken).ConfigureAwait(false))
                {
                    healthCheckResult = HealthCheckResult.Healthy();
                }
                else
                {
                    healthCheckResult = HealthCheckResult.Unhealthy();
                }
            }
            catch (Exception e)
            {
                healthCheckResult = HealthCheckResult.Unhealthy(e.Message);
            }

            return healthCheckResult;
        }
    }
}