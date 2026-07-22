using Microsoft.Azure.Cosmos;
using MS_Updates.Auth;
using MS_Updates.Func.Persistence;
using MS_Updates.Func.Rss;
using MS_Updates.Func.Utils;
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
          private readonly AzureRssService _azureRssService;
          private readonly FabricRssService _fabricRssService;
          private readonly Copilot365RssService _copilot365RssService;
          private readonly GitHubRssService _gitHubRssService;
          private readonly FoundryRssService _foundryRssService;
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

               _azureRssService = new AzureRssService(_httpClient);
               _foundryRssService = new FoundryRssService(_httpClient);
               _fabricRssService = new FabricRssService(_httpClient);
               _gitHubRssService = new GitHubRssService(_httpClient);
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
          public async Task Read_Azure_Rss_Test()
          {
               var rssItems = await _azureRssService.GetUpdatesAsync();

               // order by latest
               rssItems = rssItems.OrderByDescending(x => x.PublishedAt).Take(10).ToList();

               foreach (var item in rssItems)
               {
                    //_output.WriteLine($"{item.Title} - {item.Link}");
                    _output.WriteLine(Util.ExtractFirstSentence(item.Description));
                    _output.WriteLine("-----------------");
               }

               // save
               //await _cosmosDataService.SaveAzureUpdatesAsync(rssItems);
          }

          [Fact]
          public async Task Read_Azure_Rss_Test2()
          {
               var rssItems = await _azureRssService.GetUpdatesAsync();

               // order by latest
               rssItems = rssItems.OrderByDescending(x => x.PublishedAt).Take(10).ToList();

               foreach (var item in rssItems)
               {
                    //_output.WriteLine($"{item.Title} - {item.Link}");
                    _output.WriteLine(item.Description);
                    _output.WriteLine("-----------------");
               }

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
               var rssItems = await _fabricRssService.GetUpdatesAsync();
               rssItems = rssItems.OrderByDescending(x => x.PublishedAt).Take(10).ToList();

               foreach (var item in rssItems)
               {
                    _output.WriteLine($"{item.Id}: {Util.ExtractFirstParagraph(item.DescriptionHtml)}");
               }

               // save to cosmos
               //await _cosmosDataService.SaveFabricUpdatesAsync(rssItems);
          }

          [Fact]
          public async Task Read_GitHub_Rss_Test()
          {
               var rssItems = await _gitHubRssService.GetLatestAsync();
               foreach (var item in rssItems)
               {
                    _output.WriteLine($"{item.Title} - {item.Url}");
               }
               // save to cosmos
               //await _cosmosDataService.SaveGitHubUpdatesAsync(rssItems);
          }
     }
}
