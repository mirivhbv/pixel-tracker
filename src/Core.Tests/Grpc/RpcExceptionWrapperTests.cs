using Core.Grpc;
using Grpc.Core;

namespace Core.Tests.Grpc;

[TestFixture]
public class RpcExceptionWrapperTests
{
    [Test]
    public void Unwrap_Cancelled_ShouldUnwrapToOperationCancelledException()
    {
        // Arrange
        var sut = new RpcExceptionWrapper();
        var exception = new OperationCanceledException("Operation was cancelled");
        var rpcException = sut.Wrap(exception);

        // Act
        var result = sut.Unwrap(rpcException);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<OperationCanceledException>(),
            "We reconstruct operation cancelled because this is a normal control-flow mechanism for TPL");
        Assert.That(result.Message, Is.EqualTo(exception.Message));
    }

    [Test]
    public void Unwrap_CannotDeserializeDetail_ShouldReturnOriginalException()
    {
        // Arrange
        var sut = new RpcExceptionWrapper();
        var rpcException =
            new RpcException(new Status(StatusCode.Internal, "this cannot be deserialized"));

        // Act
        var result = sut.Unwrap(rpcException);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.SameAs(rpcException));
    }

    [Test]
    // Known exception types
    [TestCase(typeof(ArgumentNullException), typeof(ArgumentNullException))]
    [TestCase(typeof(ArgumentException), typeof(ArgumentException))]
    [TestCase(typeof(InvalidOperationException), typeof(InvalidOperationException))]
    [TestCase(typeof(OperationCanceledException), typeof(OperationCanceledException))]
    // Unknown should fallback to InvalidOperation
    [TestCase(typeof(Exception), typeof(InvalidOperationException))]
    [TestCase(typeof(FormatException), typeof(InvalidOperationException))]
    public void Unwrap_Exceptions_ShouldUnwrapToExceptionTypes(Type exceptionType, Type expectedType)
    {
        // Arrange
        var sut = new RpcExceptionWrapper();
        var message = "Error message";
        var ctor = exceptionType.GetConstructor(new[] { typeof(string) });
        var exception = (Exception)ctor.Invoke(new object[] { message });
        var rpcException = sut.Wrap(exception);

        // Act
        var result = sut.Unwrap(rpcException);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf(expectedType));
        Assert.That(result.Message, Is.EqualTo(exception.Message));
    }


    [Test]
    [TestCase(typeof(ArgumentNullException))]
    [TestCase(typeof(ArgumentException))]
    [TestCase(typeof(InvalidOperationException))]
    public void Wrap_NormalException(Type exceptionType)
    {
        // Arrange
        var sut = new RpcExceptionWrapper();
        var message = "Error message";

        // Act
        var ctor = exceptionType.GetConstructor(new[] { typeof(string) });
        var exception = (Exception)ctor.Invoke(new object[] { message });
        var result = sut.Wrap(exception);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCode.Internal), "Should use Internal enum");

        Assert.That(sut.Unwrap(result).Message, Is.EqualTo(exception.Message),
            "Should still be able to get original message");
    }

    [Test]
    public void Wrap_OperationCancelledException()
    {
        // Arrange
        var sut = new RpcExceptionWrapper();

        // Act
        var exception = new OperationCanceledException("Operation was cancelled");
        var result = sut.Wrap(exception);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCode.Internal),
            "Should use Internal, so we can make assumption about the Status.Detail");
        Assert.That(sut.Unwrap(result).Message, Is.EqualTo(exception.Message),
            "Should still be able to get original message");
    }

    [Test]
    public void Wrap_RpcException_ShouldReuse()
    {
        // Arrange
        var sut = new RpcExceptionWrapper();

        // Act
        var exception = new RpcException(new Status(StatusCode.Aborted, "Message"));
        var result = sut.Wrap(exception);

        // Assert
        Assert.That(result, Is.SameAs(exception), "Expect same instance to be returned");
    }
}
