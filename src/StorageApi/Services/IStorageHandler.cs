using StorageApi.Entities;

namespace StorageApi.Services;

/// <summary>
/// The interface to save <see cref="Track"/> into the storage.
/// </summary>
public interface IStorageHandler
{
    /// <summary>
    /// Save <paramref name="track"/> into underlying storage medium.
    /// </summary>
    /// <param name="track">Track entity. (Required).</param>
    /// <returns></returns>
    Task SaveAsync(Track track);
}
