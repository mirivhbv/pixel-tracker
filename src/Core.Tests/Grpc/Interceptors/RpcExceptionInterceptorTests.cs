using Core.Grpc.Interceptors;
using Core.Grpc;
using Grpc.Core;
using Core.Contracts;
using Core.Tests.Helpers;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using ProtoBuf.Grpc.Client;
using ProtoBuf.Grpc.Server;

namespace Core.Tests.Grpc.Interceptors;

[TestFixture]
public class RpcExceptionInterceptorTests
{
    [Test]
    public async Task Unary_Exception()
    {
        // Arrange
        var sut = new RpcExceptionInterceptor(new RpcExceptionWrapper());
        var service = new FakeStorageService
        {
            StoreAsyncCallback = (_) => throw new InvalidOperationException("Something went wrong!")
        };

        // 2. Self-hosted server
        var fixture = SetupFixture(sut, service);

        // 3. Create channel
        var channel = CreateGrpcChannel(fixture.Client);
        var invoker = channel.Intercept(sut);
        var client = invoker.CreateGrpcService<IStorageService>();

        // Act
        // Assert
        Assert.That(() => client.StoreAsync(new TrackEventRequest()),
            Throws.InvalidOperationException);

        await channel.ShutdownAsync();
        fixture.Dispose();
    }

    [Test]
    public async Task Unary_HappyDays()
    {
        // Arrange
        var sut = new RpcExceptionInterceptor(new RpcExceptionWrapper());
        var service = new FakeStorageService
        {
            StoreAsyncCallback = (_) => Task.CompletedTask
        };

        // 2. Self-hosted server
        var fixture = SetupFixture(sut, service);

        // 3. Create channel
        var channel = CreateGrpcChannel(fixture.Client);
        var invoker = channel.Intercept(sut);
        var client = invoker.CreateGrpcService<IStorageService>();

        // Act
        // Assert
        Assert.DoesNotThrowAsync(() => client.StoreAsync(new TrackEventRequest { Date = DateTime.UtcNow, IpAddress = "127.0.10.1", Referer = "", UserAgent = "" }));

        await channel.ShutdownAsync();
        fixture.Dispose();
    }

    private static ChannelBase CreateGrpcChannel(HttpClient httpClient)
    {
        ChannelBase clientChannel = GrpcChannel.ForAddress(httpClient.BaseAddress!,
            new GrpcChannelOptions
            {
                HttpClient = httpClient
            });
        return clientChannel;
    }

    /// <summary>
    /// Setup the fixture.
    /// </summary>
    private static GrpcTestFixture<TestStartup<IStorageService>>
        SetupFixture(RpcExceptionInterceptor sut, IStorageService service)
    {
        return new GrpcTestFixture<TestStartup<IStorageService>>(services =>
        {
            // Setup: Register interceptor
            services.AddSingleton(sut);

            // Setup: Configure interceptor
            services.AddCodeFirstGrpc(opt => { opt.Interceptors.Add(sut.GetType()); });

            // Setup: Register grpc service
            services.AddSingleton(service);
        });
    }
}