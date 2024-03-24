using Core.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using StorageApi.Services;

namespace StorageApi.Tests.Services;

public class FilesystemStorageHandlerTests
{
    public string? TestTempPath { get; private set; }

    [SetUp]
    public void Setup()
    {
        var currentFolder = Directory.GetCurrentDirectory();
        TestTempPath = Path.Combine(currentFolder, "tests.log");
    }

    [TearDown]
    public void Teardown()
    {
        if (File.Exists(TestTempPath))
        {
            File.Delete(TestTempPath);
        }
    }

    [Test]
    public void Constructor_ForNullLogger_ThrowsArgumentNullException()
    {
        // Arrange
        var actual = Assert.Throws<ArgumentNullException>(() => _ = new FilesystemStorageHandler(null!, null!));

        // Act
        // Assert
        Assert.That(actual?.ParamName, Is.EqualTo("logger"));
    }


    [Test]
    public void Constructor_ForNullConfiguration_FallbackPathIsUsed()
    {
        // Arrange
        var sut = new FilesystemStorageHandler(null!, new NullLogger<FilesystemStorageHandler>());

        // Act
        // Assert
        Assert.That(sut.StoragePath, Is.EqualTo("/tmp/visits.log"));
    }

    [Test]
    public void Constructor_ForAbsentConfigurationKey_FallbackPathIsUsed()
    {
        // Arrange
        const string expected = "/test/ing.log";
        var configs = new Dictionary<string, string>
        {
            {"StoragePath", expected},
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configs!)
            .Build();

        // Act
        var actual = new FilesystemStorageHandler(configuration, new NullLogger<FilesystemStorageHandler>()).StoragePath;

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void SaveAsync_ForNullTrackParameter_ThrowsArgumentNullException()
    {
        // Arrange
        var sut = new FilesystemStorageHandler(null!, new NullLogger<FilesystemStorageHandler>());

        // Act
        // Assert
        var actual = Assert.ThrowsAsync<ArgumentNullException>(() => sut.SaveAsync(null!));
        Assert.That(actual?.ParamName, Is.EqualTo("track"));
    }

    [Test]
    public void SaveAsync_ForNullTrackIpAddressProperty_ThrowsArgumentException()
    {
        // Arrange
        var sut = new FilesystemStorageHandler(null!, new NullLogger<FilesystemStorageHandler>());

        // Act
        // Assert
        var actual = Assert.ThrowsAsync<ArgumentException>(() => sut.SaveAsync(new TrackEvent()));
        Assert.That(actual?.ParamName, Is.EqualTo(nameof(TrackEvent.IpAddress)));
    }

    [Test]
    public async Task SaveAsync_ShouldSaveTrackToFile()
    {
        // Arrange
        var configs = new Dictionary<string, string>
        {
            {"StoragePath", TestTempPath!},
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configs!)
            .Build();
        var track = new TrackEvent
        {
            Date = DateTime.UtcNow,
            IpAddress = "127.0.0.10",
            Referer = "https://google.com",
            UserAgent = "postman/1.0"
        };
        var sut = new FilesystemStorageHandler(configuration, new NullLogger<FilesystemStorageHandler>());

        // Act
        await sut.SaveAsync(track);

        // Assert
        var lines = await File.ReadAllLinesAsync(TestTempPath!);
        Assert.That(lines, Has.Length.EqualTo(1));
        Assert.That(lines, Does.Contain(track.ToString()));
    }

    [Test]
    [TestCaseSource(nameof(TrackTestCases))]
    public async Task SaveAsync_ShouldSaveInCorrectFormat((TrackEvent track, string expected) pair)
    {
        // Arrange
        var track = pair.track;
        var expected = pair.expected;
        var configs = new Dictionary<string, string>
        {
            {"StoragePath", TestTempPath!},
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configs!)
            .Build();
        var sut = new FilesystemStorageHandler(configuration, new NullLogger<FilesystemStorageHandler>());

        // Act
        await sut.SaveAsync(track);

        // Assert
        var lines = await File.ReadAllLinesAsync(TestTempPath!);
        Assert.That(lines, Does.Contain(expected));
    }

    [Test]
    public async Task SaveAsync_FileWriteIsThreadSafe()
    {
        // Arrange
        const int expected = 250;
        var configs = new Dictionary<string, string>
        {
            {"StoragePath", TestTempPath!},
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configs!)
            .Build();
        var sut = new FilesystemStorageHandler(configuration, new NullLogger<FilesystemStorageHandler>());
        var runningTasks = new List<Task>();

        // Act
        var track = new TrackEvent
        {
            Date = DateTime.UtcNow,
            IpAddress = "127.0.0.10",
            Referer = "https://google.com",
            UserAgent = "postman/1.0"
        };
        for (var i = 0; i < expected; i++)
        {
            runningTasks.Add(Task.Run(() => sut.SaveAsync(track)));
        }

        await Task.WhenAll(runningTasks);

        // Assert
        var lines = await File.ReadAllLinesAsync(TestTempPath!);
        Assert.That(lines, Has.Length.EqualTo(expected));
    }

    private static IEnumerable<(TrackEvent, string)> TrackTestCases()
    {
        var date = DateTime.UtcNow;
        yield return (new TrackEvent { Date = date, Referer = "https://google.com", UserAgent = "postman/1.0", IpAddress = "127.0.0.10" }, $"{date:yyyy-MM-ddTHH:mm:ss.fffZ}|https://google.com|postman/1.0|127.0.0.10");
        yield return (new TrackEvent { Date = date, Referer = "   ", UserAgent = "          ", IpAddress = "127.0.0.10" }, $"{date:yyyy-MM-ddTHH:mm:ss.fffZ}|   |          |127.0.0.10");
        yield return (new TrackEvent { Date = date, Referer = "", UserAgent = "", IpAddress = "127.0.0.10" }, $"{date:yyyy-MM-ddTHH:mm:ss.fffZ}|null|null|127.0.0.10");
        yield return (new TrackEvent { Date = date, Referer = null, UserAgent = null, IpAddress = "127.0.0.10" }, $"{date:yyyy-MM-ddTHH:mm:ss.fffZ}|null|null|127.0.0.10");
    }
}
