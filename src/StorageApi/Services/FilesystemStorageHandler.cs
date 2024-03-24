using StorageApi.Entities;

namespace StorageApi.Services;

/// <summary>
/// The storage handler to save files into local filesystem.
/// </summary>
public class FilesystemStorageHandler : IStorageHandler
{
    private readonly ILogger<FilesystemStorageHandler> _logger;

    private const string FallbackPath = "/tmp/visits.log";

    private static readonly SemaphoreSlim Semaphore = new(1, 1);

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
    /// Appends <paramref name="track"/> to file.
    /// </remarks>
    /// <exception cref="ArgumentException"><see cref="Track.IpAddress"/> is null or empty.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="track"/> is null.</exception>
    public async Task SaveAsync(Track track)
    {
        if (track == null)
        {
            _logger.LogWarning("Track event is null or empty.");
            throw new ArgumentNullException(nameof(track));
        }

        // IP Address is mandatory.
        if (string.IsNullOrEmpty(track.IpAddress))
        {
            _logger.LogWarning("Track entity's IpAddress is null or empty.");
            throw new ArgumentException("Cannot be neither null or empty", nameof(track.IpAddress));
        }

        await Semaphore.WaitAsync();
        try
        {
            await File.AppendAllLinesAsync(StoragePath, [track.ToString()]);
        }
        finally
        {
            Semaphore.Release();
        }

        _logger.LogInformation($"Track stored: {track}");
    }
}