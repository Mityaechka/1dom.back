using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Taxiverk.Infrastructure.CacheService;

namespace Taxiverk.Infrastructure.CacheWrapper;

public class CacheWrapper : ICacheWrapper
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<CacheWrapper> _logger;

    public CacheWrapper(ICacheService cacheService, ILogger<CacheWrapper> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }
    public async Task<T> Wrap<T>(string key, string query, Func<Task<T>> request) where T : class
    {
        var cachedResponse = await _cacheService.GetValue<T>(key + ":" + query);

        if (cachedResponse != null)
        {
            _logger.Log(LogLevel.Information,"Key: {0}; Value: {1}", key + ":" + query, JsonConvert.SerializeObject(cachedResponse));
            
            return cachedResponse;
        }

        var reposne = await request();

        if (reposne != null)
        {
            await _cacheService.SetValue(key + ":" + query, reposne, TimeSpan.FromMinutes(6000));
        }

        return reposne;
    }
}