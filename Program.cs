using System.Text;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public class Program
{
    private static IMongoCollection<ReportsDataSighting> collection; // Declare collection as a class-level variable
    private static MongoClient client; // Declare MongoClient as a class-level variable

    private static void Main(string[] args)
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appSettings.json");
        var config = builder.Build();

        // Read configuration values from appSettings.json
        var connectionString = config.GetSection("Database:ConnectionString").Value;
        var databaseName = config.GetSection("Database:Name").Value;

        // Create a MongoClient instance using the connection string
        client = new MongoClient(connectionString);

        // Access the MongoDB database
        var database = client.GetDatabase(databaseName);

        // Access the collection where your data is stored
        collection = database.GetCollection<ReportsDataSighting>(ReportsDataSighting.CollectionName);

        var factory = new ConnectionFactory { Uri = new Uri("amqp://guest:guest@localhost:5672") };
        using (var connection = factory.CreateConnection())
        {
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare("your_queue",
                    false,
                    false,
                    false,
                    null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine("Received message: " + message);
                   
                    // Deserialize the received JSON message to extract query parameters
                    var queryParameters = JsonConvert.DeserializeObject<QueryParameters>(message);
                    
                    // Construct a LINQ query to fetch data based on the criteria
                    var queryResult = QueryMongoDb(queryParameters);
                    
                    // todo Calculate visitors
                    
                    
                    // Serialize the result into JSON
                    var jsonResult = JsonConvert.SerializeObject(queryResult);
                    Console.WriteLine(jsonResult);
                    // Respond back to the sender (e.g., by publishing to a response queue)
                    var responseChannel = connection.CreateModel();
                    responseChannel.QueueDeclare("response_queue", false, false, false, null);
                    var responseMessage = Encoding.UTF8.GetBytes(jsonResult);
                    responseChannel.BasicPublish("", "response_queue", null, responseMessage);
                };
                channel.BasicConsume("your_queue", true, consumer);

                Console.WriteLine("Press [Enter] to exit.");
                Console.ReadLine();
            }
        }
    }

    private static IQueryable<ReportsDataSighting> QueryMongoDb(QueryParameters parameters)
    {
        Console.WriteLine(JsonConvert.SerializeObject(parameters));
        
        // Construct a LINQ query to fetch data based on multiple cameras and the selected date
        var query = from sighting in collection.AsQueryable()
            where parameters.Cameras.Contains(sighting.Camera) &&
                  sighting.Timestamp >= GetTimeStamp(parameters.SelectedDay) &&
                  sighting.Timestamp < GetTimeStamp(parameters.SelectedDay.AddDays(1))
            select sighting;
        
        return query;
    }

    private static long GetTimeStamp(DateTime dateTime)
    {
        // Calculate the Unix timestamp (milliseconds)
        return (long)(dateTime - new DateTime(1970, 1, 1)).TotalMilliseconds;
    }

    // Define a class for query parameters
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
}