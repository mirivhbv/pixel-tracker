namespace StorageApi.Entities;

/// <summary>
/// Represents track entity.
/// </summary>
public class Track
{
    /// <summary>
    /// Date of track recorded.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Referer header value.
    /// </summary>
    public string? Referer { get; set; }

    /// <summary>
    /// UserAgent header value.
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// IP Address of the origin.
    /// </summary>
    public string? IpAddress { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Date}|{(string.IsNullOrEmpty(Referer) ? "null" : Referer)}|{(string.IsNullOrEmpty(UserAgent) ? "null" : UserAgent)}|{(string.IsNullOrEmpty(IpAddress) ? "null" : IpAddress)}";
    }
}