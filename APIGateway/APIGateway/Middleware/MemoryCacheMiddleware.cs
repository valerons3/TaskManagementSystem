using System.Security.Claims;
using Microsoft.Extensions.Caching.Memory;

namespace APIGateway.Middleware;

public class MemoryCacheMiddleware
{
    private readonly RequestDelegate next;
    private readonly IMemoryCache cache;

    private record CachedResponse(string Body, int StatusCode);

    public MemoryCacheMiddleware(RequestDelegate next, IMemoryCache cache)
    {
        this.next = next;
        this.cache = cache;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var method = context.Request.Method;

        if (method is "POST" or "PUT" or "DELETE")
        {
            string cacheKey = BuildCacheKey(context);
            cache.Remove(cacheKey);
            await next(context);
            return;
        }

        if (method != "GET")
        {
            await next(context);
            return;
        }

        string key = BuildCacheKey(context);

        if (cache.TryGetValue(key, out CachedResponse? cached))
        {
            context.Response.StatusCode = cached.StatusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(cached.Body);
            return;
        }

        var originalBody = context.Response.Body;
        using var memoryStream = new MemoryStream();
        context.Response.Body = memoryStream;

        await next(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var bodyText = await new StreamReader(context.Response.Body).ReadToEndAsync();

        var statusCode = context.Response.StatusCode;

        if (statusCode == 200)
        {
            cache.Set(key, new CachedResponse(bodyText, statusCode), TimeSpan.FromSeconds(30));
        }

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        await context.Response.Body.CopyToAsync(originalBody);
        context.Response.Body = originalBody;
    }

    private string BuildCacheKey(HttpContext context)
    {
        var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anon";
        return $"{userId}:{context.Request.Path}{context.Request.QueryString}";
    }
}