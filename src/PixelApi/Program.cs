using Grpc.Net.Client;
using ProtoBuf.Grpc.Client;
using ProtoContract.Contracts;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseHttpsRedirection();

app.UseDeveloperExceptionPage();

app.MapGet("/", async (HttpRequest request) =>
{
    // todo: is it 100% sure way of to grab IP?
    var ipAddress = request.HttpContext.Connection.RemoteIpAddress?.ToString();
    var userAgent = request.Headers["User-Agent"].ToString();
    var referer = request.Headers["Referer"].ToString();

    using var channel = GrpcChannel.ForAddress("http://storageapi:5201");
    var client = channel.CreateGrpcService<IStorageService>();

    await client.StoreAsync(
        new TrackingRequest { Date = DateTime.Now, IPAddress = ipAddress, UserAgent = userAgent, Referer = referer });
});

app.Run();
