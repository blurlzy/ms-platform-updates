using Microsoft.Azure.Cosmos;
using MS_Updates.Models;

namespace MS_Updates.Persistence
{
     public class CosmosDataService : CosmosDbContext<CosmosItem>
     {
          // ctor
          public CosmosDataService(string connectionString, string database, string container) : base(connectionString, database, container)
          {

          }

          public async Task<PagedList<CosmosItem>> ListUpdatesAsync(int pageIndex, int pageSize)
          {
               var q = @"SELECT * FROM c ORDER BY c.publishedAt DESC OFFSET @skip LIMIT @take";

               QueryDefinition query = new QueryDefinition(q)
                   .WithParameter("@skip", pageIndex * pageSize)
                   .WithParameter("@take", pageSize);
               var resultsets = await base.RunQueryAsync(query);

               var countQ = @"SELECT VALUE COUNT(1) FROM c";
               QueryDefinition countQuery = new QueryDefinition(countQ);
               var totalCount = await base.CountAsync(countQuery);

               return new PagedList<CosmosItem>(totalCount, resultsets);
          }

          public async Task<PagedList<CosmosItem>> ListUpdatesAsync(string source, int pageIndex, int pageSize)
          {
               var q = @"SELECT * FROM c WHERE LOWER(c.source) = @source ORDER BY c.publishedAt DESC OFFSET @skip LIMIT @take";

               QueryDefinition query = new QueryDefinition(q)
                   .WithParameter("@source", source.ToLower())
                   .WithParameter("@skip", pageIndex * pageSize)
                   .WithParameter("@take", pageSize);
               var resultsets = await base.RunQueryAsync(query);

               var countQ = @"SELECT VALUE COUNT(1) FROM c WHERE LOWER(c.source) = @source";
               QueryDefinition countQuery = new QueryDefinition(countQ)
                   .WithParameter("@source", source.ToLower());
               var totalCount = await base.CountAsync(countQuery);

               return new PagedList<CosmosItem>(totalCount, resultsets);
          }

          public async Task<PagedList<CosmosItem>> SearchUpdatesAsync(string keyword, int pageIndex, int pageSize)
          {
               var q = @"SELECT * FROM c
                         WHERE CONTAINS(LOWER(c.title), @keyword, true)
                            OR CONTAINS(LOWER(c.description), @keyword, true)
                            OR CONTAINS(LOWER(c.source), @keyword, true)
                            OR ARRAY_CONTAINS(c.categories, @keyword)
                         ORDER BY c.publishedAt DESC OFFSET @skip LIMIT @take";

               QueryDefinition query = new QueryDefinition(q)
                   .WithParameter("@keyword", keyword.ToLower())
                   .WithParameter("@skip", pageIndex * pageSize)
                   .WithParameter("@take", pageSize);
               var resultsets = await base.RunQueryAsync(query);

               var countQ = @"SELECT VALUE COUNT(1) FROM c
                              WHERE CONTAINS(LOWER(c.title), @keyword, true)
                                 OR CONTAINS(LOWER(c.description), @keyword, true)
                                 OR CONTAINS(LOWER(c.source), @keyword, true)
                                 OR ARRAY_CONTAINS(c.categories, @keyword)";
               QueryDefinition countQuery = new QueryDefinition(countQ)
                   .WithParameter("@keyword", keyword.ToLower());
               var totalCount = await base.CountAsync(countQuery);

               return new PagedList<CosmosItem>(totalCount, resultsets);
          }

     }
}
