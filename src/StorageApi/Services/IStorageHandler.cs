using StorageApi.Models;

namespace StorageApi.Services;

// TODO: comment
public interface IStorageHandler
{
    Task SaveAsync(Track track);
}
