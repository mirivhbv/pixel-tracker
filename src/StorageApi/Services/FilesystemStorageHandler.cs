using StorageApi.Models;

namespace StorageApi.Services;

public class FilesystemStorageHandler : IStorageHandler
{
    private readonly string _storagePath;
    private readonly ILogger<FilesystemStorageHandler> _logger;

    private const string FallbackPath = "/tmp/visits.log";

    public FilesystemStorageHandler(IConfiguration configuration, ILogger<FilesystemStorageHandler> logger)
    {
        _storagePath = configuration["StoragePath"] ?? FallbackPath;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger)); // TODO: need?
    }

    public async Task SaveAsync(Track track)
    {
        if (string.IsNullOrEmpty(track.IPAddress))
        {
            _logger.LogWarning("Event's IPAddress is null or empty");
            throw new ArgumentException("Cannot be neither null or empty", nameof(track.IPAddress));
        }

        if (string.IsNullOrEmpty(track.Referer)) track.Referer = "null";
        if (string.IsNullOrEmpty(track.UserAgent)) track.UserAgent = "null";

        var entry = $"{track.Date}|{track.Referer}|{track.UserAgent}|{track.IPAddress}";
        await File.AppendAllLinesAsync(_storagePath, [entry]);

        _logger.LogInformation($"Track stored: {entry}");
    }
}