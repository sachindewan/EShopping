using Basket.Application.Handlers;
using Basket.Application.Services;
using Basket.Core.Repositories;
using Basket.Infrastructure.Repositories;
using Discount.Grpc.Protos;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddDistributedMemoryCache();
//builder.AddRedisDistributedCache("distributedCache", setting =>
//{
//    setting.Tracing = true;
//});
builder.Services.AddMediatR(src => src.RegisterServicesFromAssembly(typeof(CreateShoppingCartCommandHandler).GetTypeInfo().Assembly));
builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo() { Title = "Basket.API", Version = "v1" });
});

builder.Services.AddControllers();
builder.Services.AddScoped<DiscountGrpcService>();
builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(client =>
{
    client.Address = new(builder.Configuration["GrpcSettings:DiscountUrl"] ?? throw new InvalidOperationException("invalid configuration"));
});
// Add services to the container.

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Basket.API"));

app.MapDefaultEndpoints();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();