using Core.Contracts;

namespace Core.Tests.Grpc.Interceptors;

/// <summary>
/// Fake implementation of <see cref="IStorageService" />.
/// </summary>
public class FakeStorageService : IStorageService
{
    /// <summary>
    /// Custom callback for <see cref="StoreAsync" />.
    /// </summary>
    public required Func<TrackEventRequest, Task> StoreAsyncCallback { get; set; }

    /// <summary>
    /// Unary
    /// </summary>
    public Task StoreAsync(TrackEventRequest request)
    {
        return StoreAsyncCallback(request);
    }
}