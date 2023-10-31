using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appSettings.json");
        var config = builder.Build();

        // Define your MongoDB connection string and database name
        var connectionString = config.GetSection("Database:ConnectionString").Value;
        var databaseName = config.GetSection("Database:Name").Value;


        // Create a MongoClient instance using the connection string
        var client = new MongoClient(connectionString);

        // Access the MongoDB database
        var database = client.GetDatabase(databaseName);

        // Access the collection where your data is stored
        var collection = database.GetCollection<ReportsDataSighting>(ReportsDataSighting.CollectionName);

        // Define the ObjectId for the camera and the selected date
        var cameraObjectId = ObjectId.Parse("64354cb3da408e8ff468a238");
        var selectedDay = new DateTime(2023, 5, 12);

        // Construct a LINQ query to fetch data based on the criteria
        var query = from sighting in collection.AsQueryable()
            where sighting.Camera == cameraObjectId &&
                  sighting.Timestamp >= GetTimeStamp(selectedDay) &&
                  sighting.Timestamp < GetTimeStamp(selectedDay.AddDays(1))
            select sighting;

        // Execute the query and retrieve the results
        var result = query.ToList();

        // Display the results
        Console.WriteLine("Query Results:");
        foreach (var sighting in result) Console.WriteLine(sighting.ToJson());

        Console.WriteLine("Press Enter to exit.");
        Console.ReadLine();
    }

    private static long GetTimeStamp(DateTime dateTime)
    {
        // Calculate the Unix timestamp (milliseconds)
        return (long)(dateTime - new DateTime(1970, 1, 1)).TotalMilliseconds;
    }
}

public class ReportsDataSighting
{
    public const string CollectionName = "reports_data_sightings";

    [BsonElement("_id")] public ObjectId Id { get; set; }

    [BsonElement("identity")] public ObjectId Identity { get; set; }

    [BsonElement("camera")] public ObjectId Camera { get; set; }

    [BsonElement("timestamp")] public long Timestamp { get; set; }
    // Other properties of your Sighting model
}