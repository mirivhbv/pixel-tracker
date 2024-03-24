using Core.Messaging;

namespace StorageApi.Services;

/// <summary>
/// The interface to save <see cref="TrackEvent"/> into the storage.
/// </summary>
public interface IStorageHandler
{
    /// <summary>
    /// Save <paramref name="trackEvent"/> into underlying storage medium.
    /// </summary>
    /// <param name="trackEvent">Track entity. (Required).</param>
    /// <returns></returns>
    Task SaveAsync(TrackEvent? trackEvent);
}
