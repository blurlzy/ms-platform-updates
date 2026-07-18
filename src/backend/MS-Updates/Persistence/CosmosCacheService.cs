using Microsoft.Extensions.Caching.Hybrid;
using MS_Updates.Models;

namespace MS_Updates.Persistence
{
     public class CosmosCacheService
     {
          private readonly HybridCache _cache;
          private readonly CosmosDataService _csmosDataService;

          // cache hours
          private const int CacheHours = 1;

          // ctor
          public CosmosCacheService(HybridCache cache, CosmosDataService cosmosDataService)
          {
               _cache = cache;
               _csmosDataService = cosmosDataService;
          }

          public async Task<PagedList<CosmosItem>> ListUpdatesAsync(int pageIndex, int pageSize)
          {
               // cache key for all updates
               var cacheKey = $"all-updates:{pageIndex}_{pageSize}";

               return await _cache.GetOrCreateAsync(
                   cacheKey,
                   async token =>
                   {
                        return await _csmosDataService.ListUpdatesAsync(pageIndex, pageSize);
                   },
                   options: new HybridCacheEntryOptions
                   {
                        Expiration = TimeSpan.FromHours(CacheHours)
                   });
          }

          public async Task<PagedList<CosmosItem>> ListUpdatesAsync(string source, int pageIndex, int pageSize)
          {
               // cache key for updates by source
               var cacheKey = $"updates:{source}:{pageIndex}_{pageSize}";

               return await _cache.GetOrCreateAsync(
                   cacheKey,
                   
                   async token =>
                   {
                        return await _csmosDataService.ListUpdatesAsync(source, pageIndex, pageSize);
                   },
                   options: new HybridCacheEntryOptions
                   {
                        Expiration = TimeSpan.FromHours(CacheHours)
                   });
          }
     }
}
