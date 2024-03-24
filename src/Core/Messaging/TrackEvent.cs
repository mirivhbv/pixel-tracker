namespace Core.Messaging;

/// <summary>
/// Represents track entity.
/// </summary>
public class TrackEvent
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

    // Better would be having parser
    // and unit tests around that
    // but simplicity leave it as is
    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Date:yyyy-MM-ddTHH:mm:ss.fffZ}|{(string.IsNullOrEmpty(Referer) ? "null" : Referer)}|{(string.IsNullOrEmpty(UserAgent) ? "null" : UserAgent)}|{(string.IsNullOrEmpty(IpAddress) ? "null" : IpAddress)}";
    }
}