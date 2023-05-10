using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IO;
using ILoggerFactory = Castle.Core.Logging.ILoggerFactory;

namespace Taxiverk.Core.Filters;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
    private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
    private const int ReadChunkBufferLength = 4096;

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
        _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
    }

    public async Task Invoke(HttpContext context)
    {
        LogRequest(context.Request);
        await _next.Invoke(context);
        //await LogResponseAsync(context);
    }

    private async void LogRequest(HttpRequest request)
    {
        request.EnableBuffering();
        using (var requestStream = _recyclableMemoryStreamManager.GetStream())
        {
            await request.Body.CopyToAsync(requestStream);
            _logger.LogInformation($"QueryString: {request.QueryString}; Request Body: {ReadStreamInChunks(requestStream)}");
        }
    }

    private async Task LogResponseAsync(HttpContext context)
    {
        var originalBody = context.Response.Body;
        using (var responseStream = _recyclableMemoryStreamManager.GetStream())
        {
            context.Response.Body = responseStream;
            await _next.Invoke(context);
            await responseStream.CopyToAsync(originalBody);
            _logger.LogInformation($"Status: {context.Response.StatusCode}; QueryString: {context.Request.QueryString}; Response Body: {ReadStreamInChunks(responseStream)}");
        }

        context.Response.Body = originalBody;
    }

    private static string ReadStreamInChunks(Stream stream)
    {
        stream.Seek(0, SeekOrigin.Begin);
        string result;
        using (var textWriter = new StringWriter())
        using (var reader = new StreamReader(stream))
        {
            var readChunk = new char[ReadChunkBufferLength];
            int readChunkLength;
            //do while: is useful for the last iteration in case readChunkLength < chunkLength
            do
            {
                readChunkLength = reader.ReadBlock(readChunk, 0, ReadChunkBufferLength);
                textWriter.Write(readChunk, 0, readChunkLength);
            } while (readChunkLength > 0);

            result = textWriter.ToString();
        }

        return result;
    }
}