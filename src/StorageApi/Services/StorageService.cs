using Core.Contracts;
using StorageApi.Models;

namespace StorageApi.Services;

// todo: move it to grpc services namespace?
public class StorageService : IStorageService
{
    private readonly ILogger<StorageService> _logger;
    private readonly IStorageHandler _storageHandler;

    public StorageService(ILogger<StorageService> logger, IStorageHandler storageService)
    {
        // todo: do we need null checking?
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _storageHandler = storageService ?? throw new ArgumentNullException(nameof(storageService));
    }

    public Task StoreAsync(TrackEventRequest request)
    {
        return _storageHandler.SaveAsync(new Track {
            Date = request.Date,
            IpAddress = request.IpAddress,
            Referer = request.Referer,
            UserAgent = request.UserAgent
        });
    }
}