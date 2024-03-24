using Core.Exceptions;
using Core.Exceptions.Models;

namespace Core.Tests.Exceptions;

[TestFixture]
public class ExceptionDetailConverterTests
{
    [Test]
    public void ConvertFrom_NullArgument_ShouldThrowException()
    {
        // Arrange
        var sut = new ExceptionDetailConverter();

        // Act
        // Assert
        Assert.That(() => sut.ConvertFrom(null!), Throws.ArgumentNullException);
    }

    [Test]
    public void ConvertFrom_ShouldCreateExceptionDetail()
    {
        // Arrange
        var sut = new ExceptionDetailConverter();
        var message = "Message";

        Exception exception;
        try
        {
            throw new InvalidOperationException(message);
        }
        catch (Exception e)
        {
            exception = e;
        }

        // Act
        var actual = sut.ConvertFrom(exception);

        // Assert
        Assert.That(actual, Is.Not.Null);
        Assert.That(actual.Message, Is.EqualTo(message));
        Assert.That(actual.TypeName, Is.EqualTo(exception.GetType().Name));
        Assert.That(actual.StackTrace, Is.EqualTo(exception.StackTrace));
    }

    [Test]
    public void ConvertTo_InnerExceptionArgumentProvided_ShouldBeAddedAsInnerException()
    {
        // Arrange
        var sut = new ExceptionDetailConverter();
        var detail = new ExceptionDetail
        {
            TypeName = nameof(InvalidOperationException),
            Message = "Message"
        };
        var innerException = new InvalidOperationException("inner exception");

        // Act
        var actual = sut.ConvertTo(detail, innerException);

        // Assert
        Assert.That(actual, Is.InstanceOf<InvalidOperationException>());
        Assert.That(actual.InnerException, Is.SameAs(innerException));
    }

    [Test]
    [TestCase(nameof(InvalidOperationException), typeof(InvalidOperationException))]
    [TestCase(nameof(ArgumentException), typeof(ArgumentException))]
    [TestCase(nameof(ArgumentNullException), typeof(ArgumentNullException))]
    [TestCase(nameof(OperationCanceledException), typeof(OperationCanceledException))]
    [TestCase(nameof(TaskCanceledException), typeof(OperationCanceledException))]
    public void ConvertTo_KnownExceptionTypes(string typeName, Type expectedExceptionType)
    {
        // Arrange
        var sut = new ExceptionDetailConverter();
        var detail = new ExceptionDetail
        {
            TypeName = typeName,
            Message = "Message"
        };

        // Act
        var actual = sut.ConvertTo(detail, null!);

        // Assert
        Assert.That(actual, Is.InstanceOf(expectedExceptionType));
    }

    [Test]
    public void ConvertTo_NullExceptionDetailArgument_ShouldThrowException()
    {
        // Arrange
        var sut = new ExceptionDetailConverter();

        // Act + Assert
        Assert.That(() => sut.ConvertTo(null!, null!), Throws.ArgumentNullException);
    }

    [Test]
    public void ConvertTo_NullInnerExceptionArgument_ShouldNotError()
    {
        // Arrange
        var sut = new ExceptionDetailConverter();
        var detail = new ExceptionDetail
        {
            TypeName = nameof(InvalidOperationException),
            Message = "Message"
        };

        // Act
        var actual = sut.ConvertTo(detail, null!);

        // Assert
        Assert.That(actual, Is.InstanceOf<InvalidOperationException>());
        Assert.That(actual.InnerException, Is.Null);
    }

    [Test]
    [TestCase(nameof(NullReferenceException))]
    [TestCase(nameof(Exception))]
    [TestCase(nameof(IndexOutOfRangeException))]
    [TestCase("SomeInvalidException")]
    public void ConvertTo_UnknownExceptionTypes_ShouldFallbackToInvalidOperation(string typeName)
    {
        ConvertTo_KnownExceptionTypes(typeName, typeof(InvalidOperationException));
    }
}