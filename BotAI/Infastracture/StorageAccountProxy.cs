using Microsoft.Azure.Cosmos.Table;
using System.Threading.Tasks;

namespace BotAI.Infastracture
{
    public class StorageAccountProxy
    {
        public CloudTable GetCloudTable(string tableName)
        {
            CloudStorageAccount storageAcc = CloudStorageAccount.Parse(System.Environment.GetEnvironmentVariable("StorageAccountConnectionString"));
            CloudTableClient tableClient = storageAcc.CreateCloudTableClient(new TableClientConfiguration());
            var table = tableClient.GetTableReference(tableName);
            table.CreateIfNotExists();
            return table;
        }

        public async Task InsertEntity<T>(CloudTable table, T entity) where T : TableEntity
        {
            TableOperation insertOperation = TableOperation.InsertOrMerge(entity);
            await table.ExecuteAsync(insertOperation);
        }

        public TableQuery<T> GetSearhQuery<T>(string partitionKey) =>
            new TableQuery<T>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));

        public TableQuery<T> GetAllQuery<T>() =>
            new TableQuery<T>();
    }
}
