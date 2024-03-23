using System.Text.Json;
using Core.Exceptions;
using Core.Exceptions.Models;
using Grpc.Core;

namespace Core.Grpc
{
    /// <summary>
    /// Wraps/unwraps <see cref="Exception" /> into <see cref="RpcException" /> and vice versa.
    /// <para>
    /// When an exception is wrapped, we denote it via
    /// </para>
    /// <para>
    /// - status code: <see cref="StatusCode.Internal" />
    /// </para>
    /// <para>
    /// - status detail: a serialized instance of <see cref="ExceptionDetail" />
    /// </para>
    /// <para>
    /// The class will attempt to recreate the original exception type during <see cref="Unwrap" />.
    /// </para>
    /// <para>
    /// Falls back to <see cref="InvalidOperationException" /> for unknown exception types.
    /// </para>
    /// </summary>
    public class RpcExceptionWrapper
    {
        private readonly ExceptionDetailConverter _exceptionDetailConverter = new();

        /// <summary>
        /// Unwraps a <see cref="RpcException" /> into an <see cref="Exception" />
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public RpcException Wrap(Exception exception)
        {
            RpcException result;
            if (exception is RpcException rpcException)
            {
                result = rpcException;
            }
            else
            {
                // A non-rpc exception.
                // Need to wrap up the exception details.

                // Status code: Internal, so receiver can assume the contents of Status.Detail
                var statusCode = StatusCode.Internal;

                // Status details: Serialize the exception detail
                var exceptionDetail = _exceptionDetailConverter.ConvertFrom(exception);
                var detailString = JsonSerializer.Serialize(exceptionDetail);

                result = new RpcException(new Status(statusCode, detailString));
            }

            return result;
        }

        /// <summary>
        /// Unwraps a <see cref="RpcException" /> into an <see cref="Exception" />
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public Exception Unwrap(RpcException exception)
        {
            Exception result = exception;
            if (exception.StatusCode != StatusCode.Internal) return result;
            ExceptionDetail? exceptionDetail;
            try
            {
                exceptionDetail = JsonSerializer.Deserialize<ExceptionDetail>(exception.Status.Detail);
            }
            catch
            {
                // Deserialization might still fail, even with "Internal".
                // In that case, it may not be our own exception.
                // Return the original exception as-is.
                exceptionDetail = null;
            }

            if (exceptionDetail != null)
            {
                result = _exceptionDetailConverter.ConvertTo(exceptionDetail, exception);
            }

            return result;
        }
    }
}
