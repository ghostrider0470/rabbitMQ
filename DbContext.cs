using System.Data.Entity;
using RabbitMQServer.Models;

namespace RabbitMQServer;



public class AppDBContext : DbContext
{
    public AppDBContext() : base("mongodb://localhost:27017/test")
    {
    }

    // Define DbSet properties for your data models
    public DbSet<ReportsDataSightings> ReportsDataSightings { get; set; }
}
