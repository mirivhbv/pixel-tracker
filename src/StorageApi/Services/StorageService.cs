using ProtoContract.Contracts;

namespace StorageApi.Services;

public class StorageService : IStorageService
{
    private readonly ILogger<StorageService> _logger;

    public StorageService(ILogger<StorageService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task StoreAsync(TrackingRequest request)
    {
        _logger.LogInformation("Store method called");
        return Task.CompletedTask;
    }
}