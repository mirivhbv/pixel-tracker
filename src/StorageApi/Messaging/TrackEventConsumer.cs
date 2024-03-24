using System.Text;
using System.Text.Json;
using Core.Messaging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StorageApi.Services;

namespace StorageApi.Messaging;

// Might be come up with much more generic approach than tightly coupled on
// TrackEvent class. But I suppose it should be okay for the task.
public class TrackEventConsumer : IHostedService
{
    private readonly IStorageHandler _storageHandler;
    private readonly ILogger<TrackEventConsumer> _logger;
    private IConnection? _connection;
    private IModel? _channel;
    private readonly ConnectionFactory _connectionFactory;

    public TrackEventConsumer(IConfiguration configuration, IStorageHandler storageHandler, ILogger<TrackEventConsumer> logger)
    {
        if (configuration is null) throw new ArgumentNullException(nameof(configuration));
        _storageHandler = storageHandler ?? throw new ArgumentNullException(nameof(storageHandler));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _connectionFactory = new ConnectionFactory
        {
            HostName = configuration["RabbitMQ:Hostname"],
            UserName = configuration["RabbitMQ:Username"],
            Password = configuration["RabbitMQ:Password"],
        };
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _connection = _connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: "track-event-queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (ch, ea) =>
        {
            var content = Encoding.UTF8.GetString(ea.Body.ToArray());

            _logger.LogInformation($"Message received: {content}");

            _channel.BasicAck(ea.DeliveryTag, false);

            try
            {
                _storageHandler.SaveAsync(JsonSerializer.Deserialize<TrackEvent>(content));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception occurred on saving to storage.");
            }
        };

        _channel.BasicConsume("track-event-queue", false, consumer);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _channel?.Close();
        _connection?.Close();

        return Task.CompletedTask;
    }
}
