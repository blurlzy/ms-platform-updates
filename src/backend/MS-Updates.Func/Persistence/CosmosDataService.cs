using MS_Updates.Func.Rss;
using MS_Updates.Func.Utils;
using Microsoft.Azure.Cosmos;

namespace MS_Updates.Func.Persistence
{
    public sealed class CosmosDataService
    {
        private readonly Container _container;

        public CosmosDataService(Container container)
        {
            //_container = cosmosClient.GetContainer(databaseId, containerId);
            _container = container;
        }

        public async Task<IReadOnlyList<CosmosItem>> GetItemsAsync(string source, DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(source);

            if (from >= to)
            {
                throw new ArgumentException("The start date must be earlier than the end date.", nameof(from));
            }

            var query = new QueryDefinition(
                "SELECT * FROM c WHERE c.source = @source AND c.publishedAt >= @from AND c.publishedAt <= @to")
                .WithParameter("@source", source)
                .WithParameter("@from", from.ToUniversalTime().ToString("O"))
                .WithParameter("@to", to.ToUniversalTime().ToString("O"));

            using var iterator = _container.GetItemQueryIterator<CosmosItem>(query);
            var items = new List<CosmosItem>();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync(cancellationToken);
                items.AddRange(response);
            }

            return items;
        }

        // check if item exists (by rssItemId, title, and source)
        private async Task<bool> ItemExistsAsync(string rssItemId, string title, string source, CancellationToken cancellationToken)
        {
            var query = new QueryDefinition(
                 "SELECT TOP 1 VALUE c.id FROM c WHERE c.source = @source AND c.rssItemId = @rssItemId AND c.title = @title")
                 .WithParameter("@source", source)
                 .WithParameter("@rssItemId", rssItemId)
                 .WithParameter("@title", title);

            var requestOptions = new QueryRequestOptions { MaxItemCount = 1 };
            using var iterator = _container.GetItemQueryIterator<string>(query, requestOptions: requestOptions);

            if (!iterator.HasMoreResults)
            {
                return false;
            }

            var response = await iterator.ReadNextAsync(cancellationToken);
            return response.Count > 0;
        }

        // save azure updates
        public async Task SaveAzureUpdatesAsync(IEnumerable<AzureUpdate> updates, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(updates);

            foreach (var azureUpdate in updates)
            {
                if (await ItemExistsAsync(azureUpdate.Id, azureUpdate.Title, UpdateSources.Azure, cancellationToken))
                {
                    continue;
                }

                // save the new update to the db
                var item = new CosmosItem(
                      azureUpdate.Id,
                      UpdateSources.Azure,
                      azureUpdate.Link,
                      azureUpdate.Title,
                      Util.ExtractFirstSentence(azureUpdate.Description),
                      azureUpdate.Categories.ToArray(),
                      string.Empty,
                      azureUpdate.PublishedAt,
                      azureUpdate.UpdatedAt
                );

                // save into cosmos db
                await _container.CreateItemAsync(item, new PartitionKey(item.Partition), cancellationToken: cancellationToken);
            }

        }



        // save foundry updates
        public async Task SaveFoundryUpdatesAsync(IEnumerable<FoundryUpdate> updates, CancellationToken cancellationToken = default)
        {
            //// get min and max published date from the updates
            //var minPublishedDate = updates.Min(u => u.PublishedAt)?.AddDays(-1).ToUniversalTime() ?? DateTimeOffset.UtcNow;
            //var maxPublishedDate = updates.Max(m => m.PublishedAt)?.ToUniversalTime() ?? DateTimeOffset.UtcNow;
            //// load the existing updates from the db with date range
            //var existingUpdates = await GetItemsAsync(UpdateSources.Foundry, minPublishedDate, maxPublishedDate, cancellationToken);

            ArgumentNullException.ThrowIfNull(updates);

            foreach (var foundryUpdate in updates)
            {
                if (await ItemExistsAsync(foundryUpdate.Id, foundryUpdate.Title, UpdateSources.Foundry, cancellationToken))
                {
                    continue;
                }

                // save the new update to the db
                var item = new CosmosItem(
                      foundryUpdate.Id,
                      UpdateSources.Foundry,
                      foundryUpdate.Link,
                      foundryUpdate.Title,
                      Util.ExtractFirstParagraph(foundryUpdate.Description), // only save the first paragraph of the description
                      Array.Empty<string>(),
                      foundryUpdate.Creator,
                      foundryUpdate.PublishedAt,
                      foundryUpdate.UpdatedAt
                );
                // save into cosmos db
                await _container.CreateItemAsync(item, new PartitionKey(item.Partition));
            }
        }

        // save fabric updates
        public async Task SaveFabricUpdatesAsync(IEnumerable<FabricUpdate> updates, CancellationToken cancellationToken = default)
        {
            //// get min and max published date from the updates
            //var minPublishedDate = updates.Min(u => u.PublishedAt)?.AddDays(-1).ToUniversalTime() ?? DateTimeOffset.UtcNow;
            //var maxPublishedDate = updates.Max(m => m.PublishedAt)?.ToUniversalTime() ?? DateTimeOffset.UtcNow;
            //// load the existing updates from the db with date range
            //var existingUpdates = await GetItemsAsync(UpdateSources.Fabric, minPublishedDate, maxPublishedDate, cancellationToken);

            ArgumentNullException.ThrowIfNull(updates);

            foreach (var fabricUpdate in updates)
            {
                if (await ItemExistsAsync(fabricUpdate.Id, fabricUpdate.Title, UpdateSources.Fabric, cancellationToken))
                {
                    continue;
                }

                // save the new update to the db
                var item = new CosmosItem(
                      fabricUpdate.Id,
                      UpdateSources.Fabric,
                      fabricUpdate.Link,
                      fabricUpdate.Title,
                      Util.ExtractFirstParagraph(fabricUpdate.DescriptionHtml), // only save the first paragraph of the description
                      Array.Empty<string>(),
                      fabricUpdate.Creator,
                      fabricUpdate.PublishedAt,
                      fabricUpdate.CreatedAt
                );
                // save into cosmos db
                await _container.CreateItemAsync(item, new PartitionKey(item.Partition));
            }
        }

        // save ms copilot 365 updates
        public async Task SaveCopilot365UpdatesAsync(IEnumerable<Copilot365Update> updates, CancellationToken cancellationToken = default)
        {
            //// get min and max published date from the updates
            //var minPublishedDate = updates.Min(u => u.PublishedAt)?.AddDays(-1).ToUniversalTime() ?? DateTimeOffset.UtcNow;
            //var maxPublishedDate = updates.Max(m => m.PublishedAt)?.ToUniversalTime() ?? DateTimeOffset.UtcNow;
            //// load the existing updates from the db with date range
            //var existingUpdates = await GetItemsAsync(UpdateSources.Copilot365, minPublishedDate, maxPublishedDate, cancellationToken);

            ArgumentNullException.ThrowIfNull(updates);

            foreach (var copilotUpdate in updates)
            {
                // check its rss item id
                if (await ItemExistsAsync(copilotUpdate.Id, copilotUpdate.Title, UpdateSources.Copilot365, cancellationToken))
                {
                    continue;
                }
                // save the new update to the db
                var item = new CosmosItem(
                      copilotUpdate.Id,
                      UpdateSources.Copilot365,
                      copilotUpdate.Link,
                      copilotUpdate.Title,
                      Util.ExtractFirstParagraph(copilotUpdate.Description), // only save the first paragraph of the description
                      Array.Empty<string>(),
                      copilotUpdate.Creator,
                      copilotUpdate.PublishedAt,
                      copilotUpdate.UpdatedAt
                );
                // save into cosmos db
                await _container.CreateItemAsync(item, new PartitionKey(item.Partition));
            }
        }

        // save github updates
        public async Task SaveGitHubUpdatesAsync(IEnumerable<GitHubFeedItem> updates, CancellationToken cancellationToken = default)
        {
            //// get min and max published date from the updates
            //var minPublishedDate = updates.Min(u => u.PublishedAt).AddDays(-1).ToUniversalTime();
            //var maxPublishedDate = updates.Max(m => m.PublishedAt).ToUniversalTime();

            //// load the existing updates from the db with date range
            //var existingUpdates = await GetItemsAsync(UpdateSources.GitHub, minPublishedDate, maxPublishedDate, cancellationToken);

            ArgumentNullException.ThrowIfNull(updates);


            foreach (var githubUpdate in updates)
            {
                // check its rss item id
                if (await ItemExistsAsync(githubUpdate.Id, githubUpdate.Title, UpdateSources.GitHub, cancellationToken))
                {
                    continue;
                }

                // save the new update to the db
                var item = new CosmosItem(
                      githubUpdate.Id,
                      UpdateSources.GitHub,
                      githubUpdate.Url.AbsoluteUri,
                      githubUpdate.Title,
                      githubUpdate.Summary ?? string.Empty,
                      githubUpdate.Categories.ToArray(),
                      string.Empty,
                      githubUpdate.PublishedAt,
                      githubUpdate.PublishedAt
                );

                // save into cosmos db
                await _container.CreateItemAsync(item, new PartitionKey(item.Partition));
            }
        }
    }
}
