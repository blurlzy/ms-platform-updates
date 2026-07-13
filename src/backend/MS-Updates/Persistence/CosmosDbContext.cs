using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System.Linq.Expressions;
using System.Net;

namespace MS_Updates.Persistence
{
    public class CosmosDbContext<T> where T : class
    {
        // cosmos client
        private readonly CosmosClient _client;

        // cosmos db container
        protected readonly Container _container;

        // max no. of list 
        private const int MAX_LIST_COUNT = 120;

        public CosmosDbContext(string connection, string database, string container)
        {
            // Configure JsonSerializerOptions
            var options = new CosmosClientOptions
            {
                SerializerOptions = new CosmosSerializationOptions
                {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                },
            };

            this._client = new CosmosClient(connection, options);
            this._container = this._client.GetContainer(database, container);
        }

        public async Task<IEnumerable<T>> QueryEntitiesAsync(Expression<Func<T, bool>> predicate)
        {
            return await Task.Run<IEnumerable<T>>(
              () => this._container.GetItemLinqQueryable<T>(true).Where(predicate).AsEnumerable());
        }

        // list items
        public async Task<List<T>> ListItemsAsync(Expression<Func<T, bool>> predicate)
        {
            List<T> results = new List<T>();

            // LINQ query generation
            using (FeedIterator<T> iterator = _container.GetItemLinqQueryable<T>(false).Where(predicate).ToFeedIterator())
            {
                // async
                while (iterator.HasMoreResults && results.Count < MAX_LIST_COUNT)
                {
                    foreach (var item in await iterator.ReadNextAsync())
                    {
                        results.Add(item);
                    }
                }
            }

            return results;
        }

        public async Task<T> ReadAsync(string entityId, string partitionKey)
        {
            if (string.IsNullOrWhiteSpace(entityId))
            {
                throw new ArgumentOutOfRangeException(nameof(entityId), "Entity Id cannot be null or empty.");
            }

            try
            {
                var response = await this._container.ReadItemAsync<T>(entityId, new PartitionKey(partitionKey));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                throw new KeyNotFoundException($"Entity with id {entityId} not found.");
            }
        }

        public async Task<ItemResponse<T>> CreateAsync(T entity, string partitionKey)
        {
            if (string.IsNullOrWhiteSpace(partitionKey))
            {
                throw new ArgumentOutOfRangeException(nameof(entity), "Partition key cannot be null or empty.");
            }

            return await this._container.CreateItemAsync(entity, new PartitionKey(partitionKey));
        }

        public async Task<ItemResponse<T>> UpsertAsync(T entity, string partitionKey)
        {
            if (string.IsNullOrWhiteSpace(partitionKey))
            {
                throw new ArgumentOutOfRangeException(nameof(entity), "Partition key cannot be null or empty.");
            }

            return await this._container.UpsertItemAsync(entity, new PartitionKey(partitionKey));
        }

        public async Task DeleteAsync(string id, string partitionKey)
        {
            if (string.IsNullOrWhiteSpace(partitionKey))
            {
                throw new ArgumentOutOfRangeException(nameof(partitionKey), "Partition key cannot be null or empty.");
            }

            await this._container.DeleteItemAsync<T>(id, new PartitionKey(partitionKey));
        }

        // run query
        public async Task<List<T>> RunQueryAsync(QueryDefinition queryDefinition, string? continuationToken = null)
        {
            var results = new List<T>();

            using FeedIterator<T> resultSetIterator = _container.GetItemQueryIterator<T>(queryDefinition, continuationToken);

            // read data
            while (resultSetIterator.HasMoreResults && results.Count < MAX_LIST_COUNT)
            {
                FeedResponse<T> response = await resultSetIterator.ReadNextAsync();
                results.AddRange(response);

            }

            return results;
        }

        // get the count value
        public async Task<int> CountAsync(QueryDefinition queryDefinition)
        {
            int count = 0;
            using FeedIterator<int> resultSetIterator = _container.GetItemQueryIterator<int>(queryDefinition);

            // it should only contain 1 row 
            while (resultSetIterator.HasMoreResults)
            {
                var currentResultSet = await resultSetIterator.ReadNextAsync();
                foreach (var res in currentResultSet)
                {
                    count += res;
                }
            }

            return count;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._client.Dispose();
            }
        }
    }
}
