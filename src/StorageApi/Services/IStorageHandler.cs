using StorageApi.Models;

namespace StorageApi.Services;

public interface IStorageHandler
{
    Task SaveAsync(Track track);
}
