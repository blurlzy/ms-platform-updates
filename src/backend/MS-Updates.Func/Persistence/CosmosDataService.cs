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

          // save azure updates
          public async Task SaveAzureUpdatesAsync(IEnumerable<AzureUpdate> updates, CancellationToken cancellationToken = default)
          {
               // get min and max published date from the updates
               // convert to date time offset to ensure the correct time zone is used
               // if its null, set as current date time offset to ensure the correct time zone is used
               // minus 1 day to ensure we get all updates in case of time zone differences
               var minPublishedDate = updates.Min(u => u.PublishedAt)?.AddDays(-1).ToUniversalTime() ?? DateTimeOffset.UtcNow;
               var maxPublishedDate = updates.Max(m => m.PublishedAt)?.ToUniversalTime() ?? DateTimeOffset.UtcNow;

               // load the existing updates from the db with date range
               var existingUpdates = await GetItemsAsync(UpdateSources.Azure, minPublishedDate, maxPublishedDate, cancellationToken);

               // 
               foreach (var azureUpdate in updates)
               {
                    // check its rss item id
                    var existingUpdate = existingUpdates.FirstOrDefault(e => e.RssItemId == azureUpdate.Id);

                    // if rss item id exists, check if the title is the same, if it is, skip it
                    if (existingUpdate != null && existingUpdate.Title == azureUpdate.Title)
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
                    await _container.CreateItemAsync(item, new PartitionKey(item.Partition));
               }

          }

          // save foundry updates
          public async Task SaveFoundryUpdatesAsync(IEnumerable<FoundryUpdate> updates, CancellationToken cancellationToken = default)
          {
               // get min and max published date from the updates
               var minPublishedDate = updates.Min(u => u.PublishedAt)?.AddDays(-1).ToUniversalTime() ?? DateTimeOffset.UtcNow;
               var maxPublishedDate = updates.Max(m => m.PublishedAt)?.ToUniversalTime() ?? DateTimeOffset.UtcNow;
               // load the existing updates from the db with date range
               var existingUpdates = await GetItemsAsync(UpdateSources.Foundry, minPublishedDate, maxPublishedDate, cancellationToken);

               foreach (var foundryUpdate in updates)
               {
                    // check its rss item id
                    var existingUpdate = existingUpdates.FirstOrDefault(e => e.RssItemId == foundryUpdate.Id);
                    // if rss item id exists, check if the title is the same, if it is, skip it
                    if (existingUpdate != null && existingUpdate.Title == foundryUpdate.Title)
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
               // get min and max published date from the updates
               var minPublishedDate = updates.Min(u => u.PublishedAt)?.AddDays(-1).ToUniversalTime() ?? DateTimeOffset.UtcNow;
               var maxPublishedDate = updates.Max(m => m.PublishedAt)?.ToUniversalTime() ?? DateTimeOffset.UtcNow;
               // load the existing updates from the db with date range
               var existingUpdates = await GetItemsAsync(UpdateSources.Fabric, minPublishedDate, maxPublishedDate, cancellationToken);
               foreach (var fabricUpdate in updates)
               {
                    // check its rss item id
                    var existingUpdate = existingUpdates.FirstOrDefault(e => e.RssItemId == fabricUpdate.Id);

                    // if rss item id exists, check if the title is the same, if it is, skip it
                    if (existingUpdate != null && existingUpdate.Title == fabricUpdate.Title)
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
               // get min and max published date from the updates
               var minPublishedDate = updates.Min(u => u.PublishedAt)?.AddDays(-1).ToUniversalTime() ?? DateTimeOffset.UtcNow;
               var maxPublishedDate = updates.Max(m => m.PublishedAt)?.ToUniversalTime() ?? DateTimeOffset.UtcNow;
               // load the existing updates from the db with date range
               var existingUpdates = await GetItemsAsync(UpdateSources.Copilot365, minPublishedDate, maxPublishedDate, cancellationToken);
               foreach (var copilotUpdate in updates)
               {
                    // check its rss item id
                    var existingUpdate = existingUpdates.FirstOrDefault(e => e.RssItemId == copilotUpdate.Id);
                    // if rss item id exists, check if the title is the same, if it is, skip it
                    if (existingUpdate != null && existingUpdate.Title == copilotUpdate.Title)
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
               // get min and max published date from the updates
               var minPublishedDate = updates.Min(u => u.PublishedAt).AddDays(-1).ToUniversalTime();
               var maxPublishedDate = updates.Max(m => m.PublishedAt).ToUniversalTime();

               // load the existing updates from the db with date range
               var existingUpdates = await GetItemsAsync(UpdateSources.GitHub, minPublishedDate, maxPublishedDate, cancellationToken);

               foreach (var githubUpdate in updates)
               {
                    // check its rss item id
                    var existingUpdate = existingUpdates.FirstOrDefault(e => e.RssItemId == githubUpdate.Id);
                    
                    // if rss item id exists, check if the title is the same, if it is, skip it
                    if (existingUpdate != null && existingUpdate.Title == githubUpdate.Title)
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
