using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Taxiverk.Infrastructure.Http;

public class Http
{
    private readonly ILogger<Http> _logger;

    public Http(ILogger<Http> logger)
    {
        _logger = logger;
    }

    public HttpCall<T> NewCall<T>()
    {
        return new HttpCall<T>(new HttpClient(), _logger)
            .SetContentHandler(s => JsonConvert.DeserializeObject<T>(s))
            .SetStatusHandler(s => HttpResultStatus.Success);
    }
}

public enum HttpResultStatus
{
    Success,
    Error
}