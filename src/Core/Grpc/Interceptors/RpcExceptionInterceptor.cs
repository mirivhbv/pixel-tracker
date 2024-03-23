using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Core.Grpc.Interceptors;

/// <summary>
/// Intercepts gRPC server and client exceptions.
/// </summary>
public class RpcExceptionInterceptor : Interceptor
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="wrapper">Rpc exception wrapper. (Required)</param>
    public RpcExceptionInterceptor(RpcExceptionWrapper wrapper)
    {
        Wrapper = wrapper ?? throw new ArgumentNullException(nameof(wrapper));
    }

    private RpcExceptionWrapper Wrapper { get; }

    /// <inheritdoc />
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (Exception e)
        {
            throw Wrapper.Wrap(e);
        }
    }

    /// <inheritdoc />
    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        try
        {
            var call = continuation(request, context);
            return new AsyncUnaryCall<TResponse>(
                EvaluateResponse(call.ResponseAsync),
                call.ResponseHeadersAsync,
                call.GetStatus,
                call.GetTrailers,
                call.Dispose);
        }
        catch (RpcException e)
        {
            throw Wrapper.Unwrap(e);
        }
    }

    private async Task<TResponse> EvaluateResponse<TResponse>(Task<TResponse> task)
    {
        try
        {
            return await task;
        }
        catch (RpcException e)
        {
            throw Wrapper.Unwrap(e);
        }
    }
}