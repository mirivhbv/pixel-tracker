namespace StorageApi.Models;

// TODO: record?
public class Track
{
    public DateTime Date { get; set; }

    public string? Referer { get; set; }

    public string? UserAgent { get; set; }

    public string IPAddress { get; set; }
}