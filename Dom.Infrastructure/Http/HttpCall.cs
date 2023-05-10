using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Taxiverk.Infrastructure.Http;

public class HttpCall<T> : IDisposable
{

    private readonly HttpClient _client;
    private readonly ILogger _logger;
    private readonly HttpRequestMessage _requestMessage;
    private Func<HttpResponseMessage, HttpResultStatus> _statusHandler;
    private ContentHandlerDelegate<T> _contentHandler;

    public HttpCall(HttpClient client,  ILogger logger)
    {
        _requestMessage = new HttpRequestMessage();
        _client = client;
        _logger = logger;
    }

    public HttpCall<T> AddHeader(string name, string value)
    {
        _requestMessage.Headers.TryAddWithoutValidation(name, value);
        return this;
    }

    public HttpCall<T> SetBaseUrl(string url)
    {
        _client.BaseAddress = new Uri(url);
        return this;
    }
    public HttpCall<T> SetUrl(string url)
    {
        _requestMessage.RequestUri = new Uri(url);
        return this;
    }
    
    public HttpCall<T> SetUrl(Uri uri)
    {
        _requestMessage.RequestUri = uri;
        return this;
    }

    public HttpCall<T> SetMethod(HttpMethod httpMethod)
    {
        _requestMessage.Method = httpMethod;
        return this;
    }
    
    public HttpCall<T> SetJsonContent(object data)
    {
        _requestMessage.Content =
            new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
        return this;
    }


    public HttpCall<T> SetStatusHandler(Func<HttpResponseMessage, HttpResultStatus> handler)
    {
        _statusHandler = handler;
        
        return this;
    }
    
    public HttpCall<T> SetContentHandler(ContentHandlerDelegate<T> handler)
    {
        _contentHandler = handler;
        return this;
    }
    
    public async Task<HttpResult<T>> Run()
    {
        var id = Guid.NewGuid().ToString();
        var logData = new List<Action>();
        var requestContent = "";

        if (_requestMessage.Content != null)
        {
            requestContent = await _requestMessage.Content.ReadAsStringAsync();
        }
        

        logData.Add(() => _logger.Log(LogLevel.Information, 
            "{0}",
            JsonConvert.SerializeObject(new
            {
                id, 
                time = DateTime.Now,
                action = "start request",
                data = new
                {
                    url = _requestMessage.RequestUri,
                    method = _requestMessage.Method,
                    content = requestContent
                }
            })));
        try
        {
            var httpResponse = await _client.SendAsync(_requestMessage);

            var status = _statusHandler(httpResponse);

            logData.Add(() => _logger.Log(LogLevel.Information, "{0}", JsonConvert.SerializeObject(new {id, time = DateTime.Now, action = "status", data = new {status} })));
            
            if (status != HttpResultStatus.Success)
            {
                return new HttpResult<T>()
                {
                    Status = status
                };
            }

            var stringContent = await httpResponse.Content.ReadAsStringAsync();
            
            logData.Add(() => _logger.Log(LogLevel.Information, "{0}", JsonConvert.SerializeObject(new {id, time = DateTime.Now, action = "content",  data = new {content = stringContent} })));
            
            var data = _contentHandler(stringContent);

            return new HttpResult<T>()
            {
                Status = status,
                Data = data
            };
        }
        catch (Exception e)
        {
            logData.Add(() => _logger.LogError(e, "{0}", JsonConvert.SerializeObject(new {id, time = DateTime.Now, action = "error" })));
            return new HttpResult<T>
            {
                Status = HttpResultStatus.Error
            };
        }
        finally
        {
            logData.ForEach(l => l());
        }
    }

    public void Dispose()
    {
        
    }

    public HttpCall<T> AddUserAgent(string agent)
    {
        _client.DefaultRequestHeaders.UserAgent.ParseAdd(agent);

        return this;
    }
}