using Core.Contracts;
using StorageApi.Entities;

namespace StorageApi.Services;

/// <summary>
/// Server grpc service of <see cref="IStorageService"/>.
/// </summary>
/// <param name="storageHandler">Storage handler to store give track event.</param>
public class StorageService(IStorageHandler storageHandler)
    : IStorageService
{
    public Task StoreAsync(TrackEventRequest request)
    {
        // can be done through mapper, but skip it due simplicity and time.
        return storageHandler.SaveAsync(new Track {
            Date = request.Date,
            IpAddress = request.IpAddress,
            Referer = request.Referer,
            UserAgent = request.UserAgent
        });
    }
}