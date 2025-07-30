using Yarp.ReverseProxy.Forwarder;

namespace APIGateway;

public class CustomTransformer : HttpTransformer
{
    public override async ValueTask TransformRequestAsync(HttpContext context, HttpRequestMessage proxyRequest, string destinationPrefix)
    {
        await base.TransformRequestAsync(context, proxyRequest, destinationPrefix);

        if (context.Request.Headers.TryGetValue("Authorization", out var auth))
        {
            proxyRequest.Headers.TryAddWithoutValidation("Authorization", (string[])auth);
        }
    }
}