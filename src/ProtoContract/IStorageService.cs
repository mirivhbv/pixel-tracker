using System.ServiceModel;

namespace ProtoContract.Contracts;

[ServiceContract]
public interface IStorageService
{
    [OperationContract]
    Task StoreAsync(TrackingRequest request);
}