namespace Taxiverk.Infrastructure.Http;

public class HttpResult<T>
{
    public HttpResultStatus Status { get; set; }
    public T Data { get; set; }
}