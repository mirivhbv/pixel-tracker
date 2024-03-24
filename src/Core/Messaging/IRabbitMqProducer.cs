namespace Core.Messaging;

public interface IRabbitMqProducer
{
    void SendMessage(object obj);
    void SendMessage(string message);
}