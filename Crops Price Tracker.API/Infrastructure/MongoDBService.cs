using Crops_Price_Tracker.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Crops_Price_Tracker.Infrastructure
{
    public class MongoDBService
    {
        public IMongoDatabase Database { get; }

        public MongoDBService(IOptions<MongoSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            Database = client.GetDatabase(settings.Value.DatabaseName);
        }
    }
}
