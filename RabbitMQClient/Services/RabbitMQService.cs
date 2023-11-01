using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQClient.Services;

public interface IRabbitService
{
    public string SetupResponseQueue(string correlationId);
    public void SendMessage(object payload, string correlationId, string responseQueue);
    public string WaitForResponse(string correlationId, int timeout);
}

public class RabbitService : IRabbitService
{
    private ConnectionFactory _factory = new ConnectionFactory { Uri = new Uri("amqp://guest:guest@localhost:5672") };

    public string SetupResponseQueue(string correlationId)
    {
        // Create a unique reply-to queue for the response
        var responseQueueName = $"response_queue_{correlationId}";

        using (var connection = _factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.QueueDeclare(responseQueueName, false, true, false, null);
        }

        return responseQueueName;
    }

    public void SendMessage(object payload, string correlationId, string responseQueue)
    {
        var queueName = "reports_data_sighting_queue";

        using (var connection = _factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            // Set up properties for the message
            var properties = channel.CreateBasicProperties();
            properties.CorrelationId = correlationId;
            properties.ReplyTo = responseQueue;

            var serializedPayload = JsonConvert.SerializeObject(payload);
            var serializedPayloadBytes = Encoding.UTF8.GetBytes(serializedPayload);

            channel.QueueDeclare(queueName, false, false, false, null);
            channel.BasicPublish("", queueName, properties, serializedPayloadBytes);
        }
    }

    public string WaitForResponse(string correlationId, int timeout)
    {
        var responseQueueName = $"response_queue_{correlationId}";

        using (var connection = _factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            var consumer = new EventingBasicConsumer(channel);
            string response = null;

            consumer.Received += (sender, ea) =>
            {
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    var body = ea.Body.ToArray();
                    response = Encoding.UTF8.GetString(body);
                }
            };

            channel.BasicConsume(responseQueueName, true, consumer);

            // Wait for a response for a specified timeout
            var waitHandle = new AutoResetEvent(false);
            var timeoutReached = !waitHandle.WaitOne(timeout);

            if (timeoutReached)
            {
                // Handle the case when a response is not received within the timeout
                return null;
            }

            return response;
        }
    }
}