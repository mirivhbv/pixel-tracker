using System.Text;
using System.Text.Json;
using Core.Messaging;
using RabbitMQ.Client;

namespace PixelApi.Messaging;

public class TrackEventProducer : IRabbitMqProducer
{
    public TrackEventProducer(IConfiguration configuration)
    {
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        Hostname = configuration["RabbitMQ:Hostname"];
        Username = configuration["RabbitMQ:Username"];
        Password = configuration["RabbitMQ:Password"];
    }

    public string? Hostname { get; }
    public string? Username { get; }
    public string? Password { get; }

    public void SendMessage(object obj)
    {
        var message = JsonSerializer.Serialize(obj);
        SendMessage(message);
    }

    public void SendMessage(string message)
    {
        var factory = new ConnectionFactory { HostName = Hostname, UserName = Username, Password = Password };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.QueueDeclare(queue: "track-event-queue",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: "",
            routingKey: "track-event-queue",
            basicProperties: null,
            body: body);
    }
}