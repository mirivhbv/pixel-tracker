using System.Runtime.Serialization;

namespace Core.Exceptions.Models;

/// <summary>
/// Stores details for an <see cref="Exception" />.
/// </summary>
public class ExceptionDetail
{
    /// <summary>
    /// Exception message.
    /// </summary>
    [DataMember]
    public required string Message { get; set; }

    /// <summary>
    /// Stack trace.
    /// </summary>
    [DataMember]
    public string? StackTrace { get; set; }

    /// <summary>
    /// Name of the exception type.
    /// </summary>
    [DataMember]
    public required string TypeName { get; set; }
}