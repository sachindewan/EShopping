using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Basket.API.Swagger;
using Basket.Application.Handlers;
using Basket.Application.Services;
using Basket.Core.Repositories;
using Basket.Infrastructure.Repositories;
using Discount.Grpc.Protos;
using Eshopping.ServiceDefaults;
using Espire.Common.Logging.Correlation;
using EventBus.Messages.Common;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.Diagnostics.Enrichment;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddDistributedMemoryCache();
builder.AddRabbitMQ("messaging");
//builder.AddRedisDistributedCache("distributedCache", setting =>
//{
//    setting.Tracing = true;
//});
builder.Services.AddMediatR(src => src.RegisterServicesFromAssembly(typeof(CreateShoppingCartCommandHandler).GetTypeInfo().Assembly));
builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    //Enable when required
    // options.ApiVersionReader = ApiVersionReader.Combine(
    //         new HeaderApiVersionReader("X-Version"),
    //         new QueryStringApiVersionReader("api-version", "ver"),
    //         new MediaTypeApiVersionReader("ver")
    //     );
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<SwaggerDefaultValues>();
});

builder.Services.AddControllers();
builder.Services.AddScoped<DiscountGrpcService>();
builder.Services.AddScoped<CorrelationIdEnricher>();
builder.Services.AddScoped<ICorrelationIdGenerator, CorrelationIdGenerator>();
builder.Services.AddScoped<CorrelationIdMiddleware>();
builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(client =>
{
    client.Address = new(builder.Configuration["GrpcSettings:DiscountUrl"] ?? throw new InvalidOperationException("invalid configuration"));
});
// Add services to the container.

var app = builder.Build();

app.MapDefaultEndpoints();


app.UseSwagger();
var serviceProvider = builder.Services.BuildServiceProvider();
var provider = serviceProvider.GetRequiredService<IApiVersionDescriptionProvider>();
app.UseSwaggerUI(options => {

    foreach (var description in provider.ApiVersionDescriptions)
    {
        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
    }
});
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<CorrelationIdMiddleware>();
app.MapGet("/send-message", (IConnection messageConnection, IConfiguration configuration) =>
{
    const string configKeyName = "RabbitMQ:QueueName";
    string? queueName = configuration[configKeyName];

    using var channel = messageConnection.CreateModel();
    channel.QueueDeclare(queueName, exclusive: false);
    channel.BasicPublish(
        exchange: "",
        routingKey: queueName,
        basicProperties: null,
        body: JsonSerializer.SerializeToUtf8Bytes(new OrderModel
        {
            Name = $"Message from API: {Guid.NewGuid()}",
            Amount = 1000
        }));

    return Results.Ok("message sent");
});
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();