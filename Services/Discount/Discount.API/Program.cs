using Discount.API;
using Discount.API.Services;
using Discount.Application.Handlers;
using Discount.Core.Repositories;
using Discount.Infrastructure.Extensions;
using Discount.Infrastructure.Repositories;
using MediatR;
using System.Reflection;
var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

//3 using npg sql database component instead of entity framework core , using dapper here.
builder.AddNpgsqlDataSource("discountdb");
builder.Services.AddMediatR(typeof(CreateDiscountCommandHandler).GetTypeInfo().Assembly);
//builder.Services.AddScoped<ICorrelationIdGenerator, CorrelationIdGenerator>();
builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddGrpc();

var app = builder.Build();

app.MapDefaultEndpoints();


app.UseRouting();
#pragma warning disable ASP0014 // Suggest using top level route registrations
app.UseEndpoints(endpoints =>
{
    endpoints.MapGrpcService<DiscountService>();
    endpoints.MapGet("/", async context =>
    {
        await context.Response.WriteAsync(
            "Communication with gRPC endpoints must be made through a gRPC client.");
    });
});
#pragma warning restore ASP0014 // Suggest using top level route registrations
app.MigrateDatabase<Program>();
app.Run();
