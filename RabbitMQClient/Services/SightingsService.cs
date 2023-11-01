namespace RabbitMQClient.Services;

public interface ISightingsService
{
    public int GetActiveVisitors();
}

public class SightingsService : ISightingsService
{
    public int GetActiveVisitors()
    {
        // Generate a unique correlation ID
        string correlationId = Guid.NewGuid().ToString();

        // Set up a callback queue for the response
        var responseQueue = _rabbitService.SetupResponseQueue(correlationId);

        // Send the request
        _rabbitService.SendMessage(requestData, correlationId, responseQueue);

        // Wait for the response with a timeout
        var response = _rabbitService.WaitForResponse(correlationId, timeout);

        if (response != null)
        {
            // Return the response to the client
            return Ok(response);
        }
        else
        {
            // Handle the case when a response is not received within the timeout
            return StatusCode(500, "Request timeout");
        }
    }
}