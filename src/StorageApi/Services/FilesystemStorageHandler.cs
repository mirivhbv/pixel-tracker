using StorageApi.Models;

namespace StorageApi.Services;

public class FilesystemStorageHandler : IStorageHandler
{
    private readonly string _storagePath;

    private const string DefaultPath = "/tmp/visits.log";

    public FilesystemStorageHandler(IConfiguration configuration)
    {
        _storagePath = configuration["StoragePath"] ?? DefaultPath;
    }

    public async Task SaveAsync(Track track)
    {
        if (string.IsNullOrEmpty(track.IPAddress))
        {
            throw new ArgumentException("Cannot be neither null or empty", nameof(track.IPAddress));
        }

        using var writeStream = File.AppendText(_storagePath);
        await writeStream.WriteLineAsync($"{track.Date}|{track.Referer}|{track.UserAgent}|{track.IPAddress}");
    }
}