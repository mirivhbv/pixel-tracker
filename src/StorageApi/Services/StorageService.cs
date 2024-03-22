using ProtoContract.Contracts;

namespace StorageApi.Services;

public class StorageService : IStorageService
{
    public Task StoreAsync(TrackingRequest request)
    {
        return Task.CompletedTask;
    }
}