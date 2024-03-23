using System.Runtime.Serialization;

namespace Core.Contracts;

[DataContract]
public class TrackEventRequest
{
    [DataMember(Order = 1)]
    public DateTime Date { get; set; }

    [DataMember(Order = 2)]
    public string? Referer { get; set; }

    [DataMember(Order = 3)]
    public string? UserAgent { get; set; }

    [DataMember(Order = 4)]
    public string? IpAddress { get; set; }
}