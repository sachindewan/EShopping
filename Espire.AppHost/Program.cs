using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);
// Load configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true) // Load environment-specific settings
    .AddEnvironmentVariables()
    .Build();

// Get connection strings from configuration
var postgressDBConnectionString = configuration.GetConnectionString("postgressdb");
var discountDBConnectionString = configuration.GetConnectionString("discountdb");
var orderDbConnectionString = configuration.GetConnectionString("orderdb");
// Orchestration
//var postgressDB = builder.AddPostgres("postgressdb");
//var discountDB = builder.AddPostgresConnection("discountdb", discountDBConnectionString);
//var orderDb = builder.AddPostgresConnection("orderdb", orderDbConnectionString);
// var distributedCaching = builder.AddRedis("distributedCache");


//messaging support

//var rabbit = builder.AddRabbitMQContainer("messaging", password: "aspire");

var discountApi = builder.AddProject<Projects.Discount_API>("discountapi");

builder.AddProject<Projects.Basket_API>("basket.api").WithReference(discountApi);//.WithReference(distributedCaching);

builder.AddProject<Projects.Catalog_API>("catalog.api");


builder.AddProject<Projects.Order_Api>("order.api");
builder.AddProject<Projects.Aspire_RabbitMq_Consumer>("consumers");//.WithReference(rabbit);
builder.Build().Run();