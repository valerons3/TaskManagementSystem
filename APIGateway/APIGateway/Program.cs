using APIGateway;
using Polly;
using Polly.Extensions.Http;
using APIGateway.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();

builder.Services.AddHttpClient("proxy")
    .AddPolicyHandler(HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(3, TimeSpan.FromSeconds(10)));


builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(builderContext => new CustomTransformer());

var app = builder.Build();

app.UseMiddleware<MemoryCacheMiddleware>();

app.MapReverseProxy();

app.Run();