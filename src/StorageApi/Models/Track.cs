namespace StorageApi.Models;

public class Track
{
    public DateTime Date { get; set; }

    public string? Referer { get; set; }

    public string? UserAgent { get; set; }

    public string? IpAddress { get; set; }
}