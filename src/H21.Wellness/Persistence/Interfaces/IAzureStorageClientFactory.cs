using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;

namespace H21.Wellness.Persistence.Interfaces
{
    public interface IAzureStorageClientFactory
    {
        Task<CloudTable> GetCloudTableAsync(
            string tableName,
            CancellationToken cancellationToken = default);
    }
}