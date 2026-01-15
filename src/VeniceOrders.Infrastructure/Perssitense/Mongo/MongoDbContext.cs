using MongoDB.Driver;
using VeniceOrders.Domain.Entities;

namespace VeniceOrders.Infrastructure.Perssitense.Mongo
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(string connectionString)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase("VeniceOrders");
        }

        public IMongoCollection<OrderItem> OrderItems => _database.GetCollection<OrderItem>("OrderItems");
    }
}
