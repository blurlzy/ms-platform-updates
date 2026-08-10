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
          [InlineData("933bc1af-e769-43b6-ac35-cdb93e4cc471")]
          public async Task Update_Item_Test(string id)
          {
               var item = await _dataService.ReadAsync(id, id);

               // update
               item.Description = "<DIV>Spend less time configuring and troubleshooting pipelines—and more time delivering <SPAN class=\"\">insights.</SPAN> <SPAN class=\"\">The</SPAN> <SPAN class=\"\">latest</SPAN> <SPAN class=\"\">Microsoft</SPAN> <SPAN class=\"\">Fabric</SPAN> <SPAN class=\"\">Data</SPAN> <SPAN class=\"\">Factory</SPAN> <SPAN class=\"\">updates</SPAN> <SPAN class=\"\">introduce</SPAN> <SPAN class=\"\">smarter</SPAN> <SPAN class=\"\">authoring,</SPAN> <SPAN class=\"\">deeper</SPAN> <SPAN class=\"\">observability,</SPAN> <SPAN class=\"\">AI-powered</SPAN> <SPAN class=\"\">operations,</SPAN> <SPAN class=\"\">and</SPAN> <SPAN class=\"\">new</SPAN> <SPAN class=\"\">pipeline</SPAN> <SPAN class=\"\">capabilities</SPAN> <SPAN class=\"\">that</SPAN> <SPAN class=\"\">make</SPAN> <SPAN class=\"\">building</SPAN> <SPAN class=\"\">and</SPAN> <SPAN class=\"\">managing</SPAN> <SPAN class=\"\">data</SPAN> <SPAN class=\"\">workflows</SPAN> <SPAN class=\"\">faster,</SPAN> <SPAN class=\"\">easier,</SPAN> <SPAN class=\"\">and</SPAN> <SPAN class=\"\">more</SPAN> <SPAN class=\"\">scalable.</SPAN></DIV>";

               // update
               var result = await _dataService.UpsertAsync(item, id);
          }
     }
}
