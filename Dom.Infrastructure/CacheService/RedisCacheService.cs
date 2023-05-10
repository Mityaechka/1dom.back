using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Taxiverk.Infrastructure.CacheService
{
    public class RedisCacheService : ICacheService
    {
        public class Options
        {
            public const string Path = "Redis";

            public string Url { get; set; }
            public int DataBase { get; set; }
        }
        private readonly IOptions<Options> _redisOptions;
        private readonly ConnectionMultiplexer _connectionMultiplexer;

        public RedisCacheService(IOptions<Options> redisOptions)
        {
            _redisOptions = redisOptions;

            _connectionMultiplexer = ConnectionMultiplexer.Connect(new ConfigurationOptions
            {
                EndPoints = { redisOptions.Value.Url }
            });
        }

        public async Task SetValue<T>(string key, T value, TimeSpan ttl) where T : class
        {
            var db = _connectionMultiplexer.GetDatabase(_redisOptions.Value.DataBase);

            await db.StringSetAsync(key, JsonConvert.SerializeObject(value), ttl);
        }

        public async Task<bool> DeleteValue(string key)
        {
            var db = _connectionMultiplexer.GetDatabase(_redisOptions.Value.DataBase);

            var result = db.KeyDelete(key);

            return true;
        }

        public async Task<T> GetValue<T>(string key) where T : class
        {
            var db = _connectionMultiplexer.GetDatabase(_redisOptions.Value.DataBase);

            string value = await db.StringGetAsync(key);

            if (value == null)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<T>(value);
        }
    }
}
