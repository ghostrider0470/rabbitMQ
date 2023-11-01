using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class ReportsDataSighting
{
    public const string CollectionName = "reports_data_sightings";

    [BsonElement("_id")] 
    public ObjectId Id { get; set; }

    [BsonElement("identity")] 
    public ObjectId Identity { get; set; }

    [BsonElement("camera")] 
    public ObjectId Camera { get; set; }

    [BsonElement("timestamp")] 
    public long Timestamp { get; set; }
}