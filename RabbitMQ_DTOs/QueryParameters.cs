using MongoDB.Bson;

namespace RabbitMQ_DTOs;

public class QueryParameters
{
    public QueryParameters(string[] cameras, string selectedDay)
    {
        Cameras = new ObjectId[cameras.Length];
        for (var i = 0; i < cameras.Length; i++)
        {
            Cameras[i] = new ObjectId(cameras[i]);
        }

        SelectedDay = DateTime.Parse(selectedDay);
    }
    public ObjectId[] Cameras { get; set; }
    public DateTime SelectedDay { get; set; }
}