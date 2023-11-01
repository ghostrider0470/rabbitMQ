using RabbitMQ_DTOs;

namespace RabbitMQClient.Services;

public interface ISightingsService
{
    public string GetActiveVisitors(string day, params string[] cameras);
}

public class SightingsService : ISightingsService
{
    private readonly IRabbitService _rabbitService;

    public SightingsService(IRabbitService _rabbitService)
    {
        this._rabbitService = _rabbitService;
    }
    public string GetActiveVisitors(string day, params string[] cameras)
    {
        // Generate a unique correlation ID
        string correlationId = Guid.NewGuid().ToString();

        // Set up a callback queue for the response
        var responseQueue = _rabbitService.SetupResponseQueue(correlationId);

        var requestData = new QueryParameters(cameras, day);
        
        // Send the request
        _rabbitService.SendMessage(requestData, correlationId, responseQueue);

        // Wait for the response with a timeout
        var response = _rabbitService.WaitForResponse(correlationId, 6000);

        if (response != null)
            return null;
        return response;
    }
}