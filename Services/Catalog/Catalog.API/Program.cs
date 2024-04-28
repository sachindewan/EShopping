using Catalog.Application.Handlers;
using Catalog.Core.Repositories;
using Catalog.Infrastructure.Data;
using Catalog.Infrastructure.Repositories;
using Microsoft.OpenApi.Models;
using System.Reflection;
var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();


//1. In project configuration
builder.AddNpgsqlDbContext<CatalogContext>("catalog", setting =>
{
    setting.ConnectionString = "Server=localhost;User Id=postgres;Password=GoGreen@123;Database=catalog";
});

//2.Orchestration
//builder.AddNpgsqlDbContext<CatalogContext>("postgressdb");

//3 using npg sql database component instead of entity framework core , using dapper here.
//builder.AddNpgsqlDataSource("postgressdb");
builder.Services.AddMediatR(src => src.RegisterServicesFromAssembly(typeof(GetAllBrandsHandler).GetTypeInfo().Assembly));
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IBrandRepository, ProductRepository>();
builder.Services.AddScoped<ITypesRepository, ProductRepository>();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo() { Title = "Catalog.API", Version = "v1" });
});

builder.Services.AddControllers();

// Add services to the container.

var app = builder.Build();

app.MapDefaultEndpoints();



app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Basket.API"));

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.CreateDbIfNotExists();
app.Run();


public partial class Program
{

}