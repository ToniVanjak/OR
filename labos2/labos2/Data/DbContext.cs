using labos2.Models;
using MongoDB.Driver;
namespace labos2.Data

{
    public class DbContext
    {
        private readonly IMongoDatabase _database;

        public DbContext(IMongoClient mongoClient)
        {
            _database = mongoClient.GetDatabase("htpremijerliga");
        }

        public IMongoCollection<Klubovi> Klubovi => _database.GetCollection<Klubovi>("Klubovi");
    }
}
