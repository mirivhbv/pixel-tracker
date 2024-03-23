using StorageApi.Entities;

namespace StorageApi.Services;

/// <summary>
/// The storage handler to save files into local filesystem.
/// </summary>
public class FilesystemStorageHandler : IStorageHandler
{
    private readonly string _storagePath;
    private readonly ILogger<FilesystemStorageHandler> _logger;

    private const string FallbackPath = "/tmp/visits.log";

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="configuration">Reads file store path from configuration. (Required)</param>
    /// <param name="logger">Logger instance. (Required)</param>
    /// <exception cref="ArgumentNullException"></exception>
    public FilesystemStorageHandler(IConfiguration configuration, ILogger<FilesystemStorageHandler> logger)
    {
        _storagePath = configuration["StoragePath"] ?? FallbackPath;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    /// <remarks>
    /// Appends <paramref name="track"/> to file.
    /// </remarks>
    /// <exception cref="ArgumentException"><see cref="Track.IpAddress"/> is null or empty.</exception>
    public async Task SaveAsync(Track track)
    {
        if (track == null)
        {
            _logger.LogWarning("Track event is null or empty.");
            throw new ArgumentNullException(nameof(track));
        }

        if (string.IsNullOrEmpty(track.IpAddress))
        {
            _logger.LogWarning("Track entity's IpAddress is null or empty.");
            throw new ArgumentException("Cannot be neither null or empty", nameof(track.IpAddress));
        }

        var entry = $"{track.Date}|{track.Referer}|{track.UserAgent}|{track.IpAddress}";
        await File.AppendAllLinesAsync(_storagePath, [ track.ToString() ]);

        _logger.LogInformation($"Track stored: {track}");
    }
}