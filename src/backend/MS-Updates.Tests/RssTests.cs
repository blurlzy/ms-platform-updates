using Microsoft.Azure.Cosmos;
using MS_Updates.Auth;
using MS_Updates.Func.Persistence;
using MS_Updates.Func.Rss;
using Xunit.Abstractions;

namespace MS_Updates.Tests
{
     public class RssTests
     {
          private readonly string _cosmosConnection = SecretManager.GetSecret(SecretKeys.CosmosConnection);
          private readonly string _cosmosDb = SecretManager.GetSecret(SecretKeys.CosmosDb);
          private readonly string _container = SecretManager.GetSecret(SecretKeys.CosmosContainer);

          // cosmos client
          private readonly CosmosClient _client;
          // http client
          private readonly HttpClient _httpClient;

          // RSS reader
          private readonly Copilot365RssService _copilot365RssService;
          private readonly CosmosDataService _cosmosDataService;

          // output
          private readonly ITestOutputHelper _output;

          public RssTests(ITestOutputHelper output)
          {
               _output = output;

               _httpClient = new HttpClient
               {
                    Timeout = TimeSpan.FromSeconds(20)
               };

               _copilot365RssService = new Copilot365RssService(_httpClient);

               // Configure JsonSerializerOptions
               var options = new CosmosClientOptions
               {
                    SerializerOptions = new CosmosSerializationOptions
                    {
                         PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                    },
               };

               this._client = new CosmosClient(_cosmosConnection, options);
               var container = this._client.GetContainer(_cosmosDb, _container);

               _cosmosDataService = new CosmosDataService(container);
          }

          [Fact]
          public async Task Read_Copilot365_Rss_Test()
          {
               var rssItems = await _copilot365RssService.GetUpdatesAsync();
               foreach (var item in rssItems)
               {
                    _output.WriteLine($"{item.Title} - {item.Link}");
               }

               // save to cosmos
               await _cosmosDataService.SaveCopilot365UpdatesAsync(rssItems);
          }

          [Fact]
          public async Task Read_Fabric_Rss_Test()
          {
               var fabricRssService = new FabricRssService(_httpClient);
               var rssItems = await fabricRssService.GetUpdatesAsync();
               foreach (var item in rssItems)
               {
                    _output.WriteLine($"{item.Title} - {item.Link}");
               }

               // save to cosmos
               await _cosmosDataService.SaveFabricUpdatesAsync(rssItems);
          }

          [Fact]
          public async Task Read_GitHub_Rss_Test()
          {
               var gitHubRssService = new GitHubRssService(_httpClient);
               var rssItems = await gitHubRssService.GetLatestAsync();
               foreach (var item in rssItems)
               {
                    _output.WriteLine($"{item.Title} - {item.Url}");
               }
               // save to cosmos
               //await _cosmosDataService.SaveGitHubUpdatesAsync(rssItems);
          }
     }
}
