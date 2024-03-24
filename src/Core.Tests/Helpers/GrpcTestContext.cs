namespace Core.Tests.Helpers;

/// <summary>
/// Based on:
/// https://github.com/grpc/grpc-dotnet/blob/master/examples/Tester/Tests/Server/IntegrationTests/Helpers/GrpcTestContext.cs
/// </summary>
internal class GrpcTestContext<TStartup> : IDisposable where TStartup : class
{
    private readonly ExecutionContext _executionContext;

    public GrpcTestContext(GrpcTestFixture<TStartup> fixture)
    {
        _executionContext = ExecutionContext.Capture()!;
    }

    public void Dispose()
    {
        _executionContext?.Dispose();
    }
}