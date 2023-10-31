using Newtonsoft.Json;

namespace RabbitMQServer.Models;

public class ReportsDataSightings
{
    [JsonProperty(PropertyName = "_id")]
    public string Id { get; set; }
    
    [JsonProperty(PropertyName = "camera")]
    public string Camera { get; set; }
    
    [JsonProperty(PropertyName = "identity")]
    public string Identity { get; set; }
    
    [JsonProperty(PropertyName = "timestamp")]
    public string Timestamp { get; set; }
}