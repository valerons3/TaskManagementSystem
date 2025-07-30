using Microsoft.Extensions.Caching.Memory;

namespace APIGateway.Middleware;

public class MemoryCacheMiddleware
{
    private readonly RequestDelegate next;
    private readonly IMemoryCache cache;

    public MemoryCacheMiddleware(RequestDelegate next, IMemoryCache cache)
    {
        this.next = next;
        this.cache = cache;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var method = context.Request.Method;
        var cacheKey = context.Request.Path + context.Request.QueryString;

        if (method is "POST" or "PUT" or "DELETE")
        {
            cache.Remove(cacheKey);
            await next(context);
            return;
        }

        if (method != "GET")
        {
            await next(context);
            return;
        }

        if (cache.TryGetValue(cacheKey, out string? cachedResponse))
        {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(cachedResponse);
            return;
        }

        var originalBodyStream = context.Response.Body;
        using var memoryStream = new MemoryStream();
        context.Response.Body = memoryStream;

        await next(context);

        memoryStream.Seek(0, SeekOrigin.Begin);
        var responseText = new StreamReader(memoryStream).ReadToEnd();

        cache.Set(cacheKey, responseText, TimeSpan.FromSeconds(30)); // можно настроить TTL

        memoryStream.Seek(0, SeekOrigin.Begin);
        await memoryStream.CopyToAsync(originalBodyStream);
        context.Response.Body = originalBodyStream;
    }
}
