namespace RabbitMQ_DTOs;



public class ReportsDataSightingDTO
{
    public ObjectId Id { get; set; }


    public ObjectId Identity { get; set; }

   
    public string[] Camera { get; set; }

   
    public DateTime Timestamp { get; set; }
}