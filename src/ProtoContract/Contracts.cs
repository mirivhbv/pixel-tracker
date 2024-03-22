using ProtoBuf.Grpc;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading.Tasks;

namespace ProtoContract.Contracts;

// todo: record?
[DataContract]
public class TrackingRequest
{
    [DataMember(Order=1)]
    public string? Referer { get; set; }
    [DataMember(Order=2)]
    public string? UserAgent { get; set; }
    // TODO: Mandatory
    [DataMember(Order=3)]
    public string? IPAddress { get; set; }
}

[ServiceContract]
public interface IStorageService
{
    [OperationContract]
    Task StoreAsync(TrackingRequest request);
}