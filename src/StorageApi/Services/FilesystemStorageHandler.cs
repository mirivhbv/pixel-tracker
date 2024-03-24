using Core.Messaging;

namespace StorageApi.Services;

/// <summary>
/// The storage handler to save files into local filesystem.
/// </summary>
public class FilesystemStorageHandler : IStorageHandler
{
    private readonly ILogger<FilesystemStorageHandler> _logger;

    private const string FallbackPath = "/tmp/visits.log";

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="configuration">Reads file store path from configuration. If not provided, uses fallback path of '/tmp/visits.log'.</param>
    /// <param name="logger">Logger instance. (Required)</param>
    /// <exception cref="ArgumentNullException"></exception>
    public FilesystemStorageHandler(IConfiguration configuration, ILogger<FilesystemStorageHandler> logger)
    {
        StoragePath = configuration?["StoragePath"] ?? FallbackPath;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Path of the file.
    /// </summary>
    public string StoragePath { get; }

    /// <inheritdoc />
    /// <remarks>
    /// Appends <paramref name="trackEvent"/> to file.
    /// </remarks>
    /// <exception cref="ArgumentException"><see cref="TrackEvent.IpAddress"/> is null or empty.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="trackEvent"/> is null.</exception>
    public async Task SaveAsync(TrackEvent? trackEvent)
    {
        if (trackEvent == null)
        {
            _logger.LogWarning("Track event is null or empty.");
            throw new ArgumentNullException(nameof(trackEvent));
        }

        // IP Address is mandatory.
        if (string.IsNullOrEmpty(trackEvent.IpAddress))
        {
            _logger.LogWarning("Track entity's IpAddress is null or empty.");
            throw new ArgumentException("Cannot be neither null or empty", nameof(trackEvent.IpAddress));
        }

        // Note: Since switched to rabbitmq and considering
        // storage api going to be max one instance
        // we may drop locking around file appending -- rabbitmq
        // configured to consume single message at a time.
        await File.AppendAllLinesAsync(StoragePath, [trackEvent.ToString()]);

        _logger.LogInformation($"Track stored: {trackEvent}");
    }
}