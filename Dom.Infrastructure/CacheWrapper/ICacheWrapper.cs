namespace Taxiverk.Infrastructure.CacheWrapper;

public interface ICacheWrapper
{
    Task<T> Wrap<T>(string key, string query, Func<Task<T>> request) where T : class;
}