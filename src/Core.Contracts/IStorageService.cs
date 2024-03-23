using System.ServiceModel;

namespace Core.Contracts;

[ServiceContract]
public interface IStorageService
{
    [OperationContract]
    Task StoreAsync(TrackEventRequest request);
}