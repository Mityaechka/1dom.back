namespace Taxiverk.Infrastructure.CacheService
{
    public interface ICacheService
    {
        Task<T> GetValue<T>(string key) where T : class;
        Task SetValue<T>(string key, T value, TimeSpan ttl) where T : class;
        Task<bool> DeleteValue(string key);
    }
}