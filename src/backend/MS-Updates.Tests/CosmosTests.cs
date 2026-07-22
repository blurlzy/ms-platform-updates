using Microsoft.Azure.Cosmos;
using Microsoft.Identity.Client;
using MS_Updates.Auth;
using MS_Updates.Persistence;
using Xunit.Abstractions;

namespace MS_Updates.Tests
{
     public class CosmosTests
     {
          private readonly string _cosmosConnection = SecretManager.GetSecret(SecretKeys.CosmosConnection);
          private readonly string _cosmosDb = SecretManager.GetSecret(SecretKeys.CosmosDb);
          private readonly string _container = SecretManager.GetSecret(SecretKeys.CosmosContainer);

          // cosmos client
          private readonly CosmosDataService _dataService;

          // output
          private readonly ITestOutputHelper _output;

          //ctor
          public CosmosTests(ITestOutputHelper output)
          {
               _output = output;

               _dataService = new CosmosDataService(_cosmosConnection, _cosmosDb, _container);
          }

          [Theory]
          [InlineData(0, 5)]
          public async Task List_Updates_Test(int pageIndex, int pageSize)
          {
               var result = await _dataService.ListUpdatesAsync(pageIndex, pageSize);

               foreach (var item in result.Data)
               {
                    _output.WriteLine(item.Title);
               }

          }

          [Theory]
          [InlineData("Github", 0, 5)]
          public async Task List_By_Source_Test(string source, int pageIndex, int pageSize)
          {
               var result = await _dataService.ListUpdatesAsync(source, pageIndex, pageSize);
               foreach (var item in result.Data)
               {
                    _output.WriteLine($"{item.Source}: {item.Title}");
               }
          }

          [Theory]
          [InlineData("b364bc66-f91b-41ab-8bad-11374b8c06d2")]
          public async Task Update_Item_Test(string id)
          {
               var item = await _dataService.ReadAsync(id, id);

               // update
               item.Description = "<DIV>What if you could ask questions about your business data in plain language, and get trusted answers wherever you work? Discover how Fabric IQ, Power BI, Microsoft 365 Copilot, and Fabric data agents are bringing the full breadth of conversational analytics into the flow of work, turning governed business context into insights and action.</DIV>";

               // update
               var result = await _dataService.UpsertAsync(item, id);
          }
     }
}
