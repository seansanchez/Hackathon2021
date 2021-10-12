using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs.Models;
using H21.Wellness.Exceptions;
using H21.Wellness.Extensions;
using H21.Wellness.Persistence.Entities;
using Microsoft.Extensions.Logging;

namespace H21.Wellness.Persistence
{
    public abstract class BaseAzureBlobStorageRepository
    {
        private readonly IAzureStorageClientFactory _azureStorageClientFactory;
        private readonly ILogger<BaseAzureBlobStorageRepository> _logger;

        protected BaseAzureBlobStorageRepository(
            IAzureStorageClientFactory azureStorageClientFactory,
            ILogger<BaseAzureBlobStorageRepository> logger)
        {
            azureStorageClientFactory.ThrowIfNull(nameof(azureStorageClientFactory));
            logger.ThrowIfNull(nameof(logger));

            _azureStorageClientFactory = azureStorageClientFactory;
            _logger = logger;
        }

        protected async Task<BlobReference> CreateBlobAsync(
            string blobContainerName,
            string blobName,
            Stream content,
            CancellationToken cancellationToken = default)
        {
            blobContainerName.ThrowIfNullOrWhitespace(nameof(blobContainerName));
            blobName.ThrowIfNullOrWhitespace(nameof(blobName));
            content.ThrowIfNull(nameof(content));

            BlobReference blobReference;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                blobReference = await UploadBlobAsync(
                    blobContainerName: blobContainerName,
                    blobName: blobName,
                    options: new BlobUploadOptions
                    {
                        Conditions = new BlobRequestConditions
                        {
                            IfNoneMatch = new ETag("*")
                        }
                    },
                    content: content,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            catch (RequestFailedException e)
            {
                TryThrowRequestFailedException(
                    exception: e,
                    blobContainerName: blobContainerName,
                    blobName: blobName);

                throw;
            }
            finally
            {
                stopwatch.Stop();
                this._logger.LogDebug($"{nameof(CreateBlobAsync)} took {stopwatch.ElapsedMilliseconds} millisecond to complete.");
            }

            return blobReference;
        }

        protected async Task<BlobReference> UpdateBlobAsync(
            string blobContainerName,
            string blobName,
            string etag,
            Stream content,
            CancellationToken cancellationToken = default)
        {
            blobContainerName.ThrowIfNullOrWhitespace(nameof(blobContainerName));
            blobName.ThrowIfNullOrWhitespace(nameof(blobName));
            etag.ThrowIfNullOrWhitespace(nameof(etag));
            content.ThrowIfNull(nameof(content));

            BlobReference blobReference;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                blobReference = await UploadBlobAsync(
                    blobContainerName: blobContainerName,
                    blobName: blobName,
                    options: new BlobUploadOptions
                    {
                        Conditions = new BlobRequestConditions
                        {
                            IfMatch = new ETag(etag)
                        }
                    },
                    content: content,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            catch (RequestFailedException e)
            {
                TryThrowRequestFailedException(
                    exception: e,
                    blobContainerName: blobContainerName,
                    blobName: blobName);

                throw;
            }
            finally
            {
                stopwatch.Stop();
                this._logger.LogDebug($"{nameof(UpdateBlobAsync)} took {stopwatch.ElapsedMilliseconds} millisecond to complete.");
            }

            return blobReference;
        }

        protected async Task<BlobProperties> GetBlobPropertiesAsync(
            string blobContainerName,
            string blobName,
            CancellationToken cancellationToken = default)
        {
            blobContainerName.ThrowIfNullOrWhitespace(nameof(blobContainerName));
            blobName.ThrowIfNullOrWhitespace(nameof(blobName));

            BlobProperties blobProperties;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                var blobClient = await _azureStorageClientFactory.GetBlobClientAsync(blobContainerName, blobName, cancellationToken).ConfigureAwait(false);
                var response = await blobClient.GetPropertiesAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

                blobProperties = response.Value;
            }
            catch (RequestFailedException e)
            {
                TryThrowRequestFailedException(
                    exception: e,
                    blobContainerName: blobContainerName,
                    blobName: blobName);

                throw;
            }
            finally
            {
                stopwatch.Stop();
                this._logger.LogDebug($"{nameof(GetBlobPropertiesAsync)} took {stopwatch.ElapsedMilliseconds} millisecond to complete.");
            }

            return blobProperties;
        }

        protected async Task<BlobReference> GetBlobReferenceAsync(
            string blobContainerName,
            string blobName,
            CancellationToken cancellationToken = default)
        {
            blobContainerName.ThrowIfNullOrWhitespace(nameof(blobContainerName));
            blobName.ThrowIfNullOrWhitespace(nameof(blobName));

            BlobReference blobReference;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                var blobClient = await _azureStorageClientFactory.GetBlobClientAsync(blobContainerName, blobName, cancellationToken).ConfigureAwait(false);

                if (!await blobClient.ExistsAsync(cancellationToken).ConfigureAwait(false))
                {
                    throw new NotFoundException($"The specified blob with {nameof(blobContainerName)}: '{blobContainerName}' and {nameof(blobName)}: '{blobName}' could not be found.");
                }

                blobReference = new BlobReference()
                {
                    BlobContainerName = blobClient.BlobContainerName,
                    BlobName = blobClient.Name
                };
            }
            catch (RequestFailedException e)
            {
                TryThrowRequestFailedException(
                    exception: e,
                    blobContainerName: blobContainerName,
                    blobName: blobName);

                throw;
            }
            finally
            {
                stopwatch.Stop();
                this._logger.LogDebug($"{nameof(GetBlobReferenceAsync)} took {stopwatch.ElapsedMilliseconds} millisecond to complete.");
            }

            return blobReference;
        }

        protected async Task<byte[]> DownloadBlobAsync(
            string blobContainerName,
            string blobName,
            CancellationToken cancellationToken = default)
        {
            blobContainerName.ThrowIfNullOrWhitespace(nameof(blobContainerName));
            blobName.ThrowIfNullOrWhitespace(nameof(blobName));

            byte[] bytes;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                var blobClient = await _azureStorageClientFactory.GetBlobClientAsync(blobContainerName, blobName, cancellationToken).ConfigureAwait(false);

                var response = await blobClient.DownloadAsync(cancellationToken).ConfigureAwait(false);

                bytes = await response.Value.Content.ToBytesAsync().ConfigureAwait(false);
            }
            catch (RequestFailedException e)
            {
                TryThrowRequestFailedException(
                    exception: e,
                    blobContainerName: blobContainerName,
                    blobName: blobName);

                throw;
            }
            finally
            {
                stopwatch.Stop();
                this._logger.LogDebug($"{nameof(DownloadBlobAsync)} took {stopwatch.ElapsedMilliseconds} millisecond to complete.");
            }

            return bytes;
        }

        private async Task<BlobReference> UploadBlobAsync(
            string blobContainerName,
            string blobName,
            BlobUploadOptions options,
            Stream content,
            CancellationToken cancellationToken)
        {
            blobContainerName.ThrowIfNullOrWhitespace(nameof(blobContainerName));
            blobName.ThrowIfNullOrWhitespace(nameof(blobName));
            options.ThrowIfNull(nameof(options));
            content.ThrowIfNull(nameof(content));

            BlobReference blobReference;

            try
            {
                var blobClient = await _azureStorageClientFactory.GetBlobClientAsync(blobContainerName, blobName, cancellationToken).ConfigureAwait(false);

                var response = await blobClient
                    .UploadAsync(
                        content: content,
                        options: options,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

                blobReference = new BlobReference()
                {
                    BlobContainerName = blobContainerName,
                    BlobName = blobName
                };
            }
            catch (RequestFailedException e)
            {
                TryThrowRequestFailedException(
                    exception: e,
                    blobContainerName: blobContainerName,
                    blobName: blobName);

                throw;
            }

            return blobReference;
        }

        private void TryThrowRequestFailedException(
            RequestFailedException exception,
            string blobContainerName,
            string blobName)
        {
            exception.ThrowIfNull(nameof(exception));
            blobContainerName.ThrowIfNullOrWhitespace(nameof(blobContainerName));
            blobName.ThrowIfNullOrWhitespace(nameof(blobName));

            if (exception.ErrorCode == BlobErrorCode.BlobNotFound)
            {
                throw new NotFoundException($"The specified {nameof(blobContainerName)}: '{blobContainerName}' {nameof(blobName)}: '{blobName}' could not be found.", exception);
            }
            else if (exception.ErrorCode == BlobErrorCode.BlobAlreadyExists)
            {
                throw new NotFoundException($"The specified {nameof(blobContainerName)}: '{blobContainerName}' {nameof(blobName)}: '{blobName}' already exists.", exception);
            }
        }
    }
}