using System.Runtime.Serialization;

namespace ProtoContract.Contracts;

// todo: record?
[DataContract]
public class TrackingRequest
{
    // TODO: timestamp
    [DataMember(Order = 1)]
    public DateTime Date { get; set; }

    [DataMember(Order = 2)]
    public string? Referer { get; set; }

    [DataMember(Order = 3)]
    public string? UserAgent { get; set; }

    // TODO: Mandatory
    [DataMember(Order = 4)]
    public string? IPAddress { get; set; }
}
