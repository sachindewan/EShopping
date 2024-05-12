using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);
// Load configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true) // Load environment-specific settings
    .AddEnvironmentVariables()
    .Build();

var grafana = builder.AddContainer("grafana", "grafana/grafana")
    .WithVolumeMount("../grafana/config", "/etc/grafana")
    .WithVolumeMount("../grafana/dashboards", "/var/lib/grafana/dashboards")
    .WithEndpoint(containerPort: 3000, hostPort: 3000, name: "grafana-http", scheme: "http");

builder.AddContainer("prometheus", "prom/prometheus")
    .WithVolumeMount("../prometheus", "/etc/prometheus")
    .WithEndpoint(9090, hostPort: 9090);

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

//var discountApi = builder.AddProject<Projects.Discount_API>("discountapi");
var customContainer = builder.AddContainer("myappdiscountapi", "basketapi")
                             .WithHttpEndpoint(hostPort: 8080, name: "endpoint");

var discountApi_endpoint = customContainer.GetEndpoint("endpoint");

builder.AddProject<Projects.Basket_API>("basket.api")//.WithReference(discountApi_endpoint)
.WithEnvironment("GRAFANA_URL", grafana.GetEndpoint("grafana-http"));//.WithReference(distributedCaching);

builder.AddProject<Projects.Catalog_API>("catalog.api").WithEnvironment("GRAFANA_URL", grafana.GetEndpoint("grafana-http"));


builder.AddProject<Projects.Order_Api>("order.api").WithEnvironment("GRAFANA_URL", grafana.GetEndpoint("grafana-http"));
builder.AddProject<Projects.Aspire_RabbitMq_Consumer>("consumers").WithEnvironment("GRAFANA_URL", grafana.GetEndpoint("grafana-http"));//.WithReference(rabbit);
builder.AddProject<Projects.WebApplication1>("webapplication1");
builder.Build().Run();