using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Taxiverk.Domain.Mongo;

public class MongoContext
{
    private readonly IOptions<Options> _options;
    private readonly MongoClient _client;
    private readonly IMongoDatabase _database;

    public IMongoCollection<object> Test => _database.GetCollection<object>("1dom.Test");

    public class Options
    {
        public const string Path = "Mongo";
        public string Connection { get; set; }
        public string Database { get; set; }
    }

    public MongoContext(IOptions<Options> options)
    {
        _options = options;
        _client = new MongoClient(_options.Value.Connection);
        _database = _client.GetDatabase(_options.Value.Database);
    }
}