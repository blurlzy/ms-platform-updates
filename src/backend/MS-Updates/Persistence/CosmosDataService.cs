using MS_Updates.Models;

namespace MS_Updates.Persistence
{
    public class CosmosDataService: CosmosDbContext<CosmosItem>
    {
        // ctor
        public CosmosDataService(string connectionString, string database, string container) : base(connectionString, database, container)
        {
                
        }
    }
}
