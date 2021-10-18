using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Azure.Cosmos.Table;

namespace H21.Wellness.Persistence
{
    public interface IAzureStorageClientFactory
    {
        Task<CloudTable> GetCloudTableAsync(
            string tableName,
            CancellationToken cancellationToken = default);

        Task<BlobContainerClient> GetBlobContainerClientAsync(
            string blobContainerName,
            CancellationToken cancellationToken = default);

        Task<BlobClient> GetBlobClientAsync(
            string blobContainerName,
            string blobName,
            CancellationToken cancellationToken = default);
    }
}