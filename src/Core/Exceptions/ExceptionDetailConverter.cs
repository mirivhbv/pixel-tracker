using Core.Exceptions.Models;

namespace Core.Exceptions;

/// <summary>
/// Converts <see cref="Exception"/> to <see cref="ExceptionDetail"/> and vice versa. 
/// </summary>
public class ExceptionDetailConverter
{
    private readonly Dictionary<string, CreateExceptionCallback?> _exceptionFactory;

    /// <summary>
    /// Constructor.
    /// </summary>
    public ExceptionDetailConverter()
    {
        _exceptionFactory = new Dictionary<string, CreateExceptionCallback?>
        {
            [nameof(ArgumentException)] =
                (message, exception) => new ArgumentException(message, exception),

            [nameof(ArgumentNullException)] =
                (message, exception) => new ArgumentNullException(message, exception),

            [nameof(OperationCanceledException)] =
                (message, exception) => new OperationCanceledException(message, exception),

            [nameof(TaskCanceledException)] =
                (message, exception) => new OperationCanceledException(message, exception),

            [nameof(InvalidOperationException)] = CreateInvalidOperationException
        };
    }

    /// <summary>
    /// Creates an instance of <see cref="InvalidOperationException" />.
    /// </summary>
    private static Exception CreateInvalidOperationException(string message,
        Exception innerException)
    {
        return new InvalidOperationException(message, innerException);
    }

    /// <summary>
    /// Converts from <see cref="Exception" /> to <see cref="ExceptionDetail" />
    /// </summary>
    /// <param name="exception">Exception to convert. Required.</param>
    public ExceptionDetail ConvertFrom(Exception exception)
    {
        if (exception == null)
        {
            throw new ArgumentNullException(nameof(exception));
        }

        return new ExceptionDetail
        {
            Message = exception.Message,
            StackTrace = exception.StackTrace,
            TypeName = exception.GetType().Name
        };
    }

    /// <summary>
    /// Converts from <see cref="ExceptionDetail" /> to <see cref="Exception" />
    /// </summary>
    /// <param name="detail">Details to convert. Required.</param>
    /// <param name="originalException">
    /// Original exception, that will be added as <see cref="Exception.InnerException" />.
    /// Optional.
    /// </param>
    public Exception ConvertTo(ExceptionDetail detail, Exception originalException)
    {
        if (detail == null)
        {
            throw new ArgumentNullException(nameof(detail));
        }

        var message = detail.Message;
        if (!_exceptionFactory.TryGetValue(
                detail.TypeName,
                out var factory))
        {
            factory = CreateInvalidOperationException;
        }

        var stackTrace = detail.StackTrace;
        var result = factory?.Invoke(message, originalException);
        result?.Data.Add(nameof(ExceptionDetail.StackTrace), stackTrace);
        return result ?? new Exception("Cannot convert RpcException to Exception.");
    }

    /// <summary>
    /// Callback delegate to create an <see cref="Exception" />
    /// </summary>
    /// <param name="message">Exception's message</param>
    /// <param name="innerException">Inner exception</param>
    private delegate Exception CreateExceptionCallback(string message, Exception innerException);
}