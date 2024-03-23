using Core.Contracts;
using Core.Grpc;
using Core.Grpc.Interceptors;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using ProtoBuf.Grpc.Client;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// OR go with having actual 1x1 GIF in wwwroot
var gifBytes = new byte[] {
    0x47, 0x49, 0x46, 0x38, 0x39, 0x61, // GIF89a header
    0x01, 0x00, 0x01, 0x00, // Image size (1x1 pixels)
    0x80, 0x00, 0x00, 0x00, // Transparent color
    0x00, 0x00, 0x00, // Image data
    0x21, 0xF9, 0x04, 0x01, 0x00, 0x00, 0x00, 0x00, // Graphic Control Extension
    0x2C, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x02, 0x02, 0x44, 0x01, 0x00, // Image Descriptor
    0x3B // GIF Trailer
};

// This is workaround here, more appropriate approach would be
// defining rpc channel decorator or factory
// to maintain single established connection
var channel = GrpcChannel.ForAddress("http://storageapi:5201", new GrpcChannelOptions
{
    HttpHandler = new SocketsHttpHandler
    {
        // https://learn.microsoft.com/en-us/aspnet/core/grpc/performance?view=aspnetcore-8.0
        EnableMultipleHttp2Connections = true
    }
});
var invoker = channel.Intercept(new RpcExceptionInterceptor(new RpcExceptionWrapper()));
var client = invoker.CreateGrpcService<IStorageService>();

app.Lifetime.ApplicationStopping.Register(() =>
{
    // regardless of the state of the channel dispose.
    try { channel.Dispose(); }
    catch { /* intentionally left blank */ }
});


app.MapGet("/track", async (HttpRequest request) =>
{
    var ipAddress = request.HttpContext.Connection.RemoteIpAddress?.ToString();
    var userAgent = request.Headers["User-Agent"].ToString();
    var referer = request.Headers["Referer"].ToString();

    try
    {
        await client.StoreAsync(
            new TrackEventRequest { Date = DateTime.UtcNow, IpAddress = ipAddress, UserAgent = userAgent, Referer = referer });
    }
    catch (Exception ex)
    {
        app.Logger.LogWarning(ex, ex.Message);
    }

    request.HttpContext.Response.Headers.Add("Content-Type", "image/gif");
    await request.HttpContext.Response.Body.WriteAsync(gifBytes);
});

app.Run();
