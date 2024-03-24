using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Core.Tests.Helpers;

/// <summary>
/// Based on:
/// https://github.com/grpc/grpc-dotnet/blob/master/examples/Tester/Tests/Server/IntegrationTests/Helpers/GrpcTestFixture.cs
/// </summary>
internal class GrpcTestFixture<TStartup> : IDisposable where TStartup : class
{
    private readonly IHost _host;
    private readonly TestServer _server;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="initialConfigureServices">Callback to configure the services</param>
    public GrpcTestFixture(Action<IServiceCollection> initialConfigureServices)
    {
        var builder = new HostBuilder()
            .ConfigureServices(initialConfigureServices.Invoke)
            .ConfigureWebHostDefaults(webHost =>
            {
                webHost
                    .UseTestServer()
                    .UseStartup<TStartup>();
            });
        _host = builder.Start();
        _server = _host.GetTestServer();

        var messageHandler = new ResponseMessageHandler
        {
            InnerHandler = _server.CreateHandler()
        };

        var client = new HttpClient(messageHandler);
        client.BaseAddress = new Uri("http://localhost");

        Client = client;
    }

    public void Dispose()
    {
        Client.Dispose();
        _host.Dispose();
        _server.Dispose();
    }

    public HttpClient Client { get; }
}

public class ResponseMessageHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        return await base.SendAsync(request, cancellationToken);
    }
}