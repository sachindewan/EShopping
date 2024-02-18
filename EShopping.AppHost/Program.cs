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
var postgressDB = builder.AddPostgresConnection("postgressdb", postgressDBConnectionString);
var discountDB = builder.AddPostgresConnection("discountdb", discountDBConnectionString);
var orderDb = builder.AddPostgresConnection("orderdb", orderDbConnectionString);
// var distributedCaching = builder.AddRedis("distributedCache");
var discountApi = builder.AddProject<Projects.Discount_API>("discountapi").WithReference(discountDB);

builder.AddProject<Projects.Basket_API>("basket.api").WithReference(discountApi);//.WithReference(distributedCaching);

builder.AddProject<Projects.Catalog_API>("catalog.api").WithReference(postgressDB);


builder.AddProject<Projects.Order_Api>("order.api").WithReference(orderDb);


builder.Build().Run();
