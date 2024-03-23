using Core.Contracts;
using Core.Grpc;
using Core.Grpc.Interceptors;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using ProtoBuf.Grpc.Client;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.MapGet("/track", async (HttpRequest request) =>
{
    var ipAddress = request.HttpContext.Connection.RemoteIpAddress?.ToString();
    var userAgent = request.Headers["User-Agent"].ToString();
    var referer = request.Headers["Referer"].ToString();

    using var channel = GrpcChannel.ForAddress("http://storageapi:5201");
    var invoker = channel.Intercept(new RpcExceptionInterceptor(new RpcExceptionWrapper()));
    var client = invoker.CreateGrpcService<IStorageService>();

    try
    {
        await client.StoreAsync(
            new TrackEventRequest { Date = DateTime.Now, IpAddress = ipAddress, UserAgent = userAgent, Referer = referer });
    }
    catch (Exception ex)
    {
        app.Logger.LogWarning(ex, ex.Message);
    }
});

app.Run();
