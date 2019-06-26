using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace dashboard.web.Providers
{
    public class MongoConnection
    {
        private MongoClient _client;
        private IMongoDatabase _db;
        private readonly ILogger<MongoConnection> _logger;

        public MongoConnection(ILogger<MongoConnection> logger)
        {
            _logger = logger;
        }

        public void Connect()
        {
            _client = new MongoClient("mongodb://admin:admin@localhost:27017");
            _db = _client.GetDatabase("dashboard");
        }

        public IMongoCollection<T> Collection<T>(string name)
        {
            if (_db == null)
            {
                _logger.LogError($"Attempting to get collection {name} before connected");
                return null;
            }
            return _db.GetCollection<T>(name);
        }
    }
}