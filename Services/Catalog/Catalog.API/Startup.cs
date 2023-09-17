using System.Net;
using System.Reflection;
using Catalog.API.HealthCheck;
using Catalog.Application.Handlers;
using Catalog.Core.Repositories;
using Catalog.Infrastructure.Data;
using Catalog.Infrastructure.Repositories;
using HealthChecks.UI.Client;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;

namespace Catalog.API
{
    public class Startup
    {
        public IConfiguration Config { get; }

        public Startup(IConfiguration configuration)
        {
            Config = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApiVersioning(config =>
            {
                // Specify the default API Version as 1.0
                config.DefaultApiVersion = new ApiVersion(1, 0);
                // If the client hasn't specified the API version in the request, use the default API version number 
                config.AssumeDefaultVersionWhenUnspecified = true;
                // Advertise the API versions supported for the particular endpoint
                config.ReportApiVersions = true;
            });
            //services.AddHealthChecks().AddMongoDb(Config["DatabaseSettings:ConnectionString"] ?? string.Empty,
            //   "Catalog MongoDb Health Check", HealthStatus.Degraded);


            services.AddHealthChecks()
                .AddCheck<MongoHealthCheck>("MongoDBConnectionCheck", HealthStatus.Unhealthy);
            services
                .AddHealthChecksUI(options =>
                {
                    var provider = services.BuildServiceProvider();

                    var env = provider.GetRequiredService<IWebHostEnvironment>();
                    //connect to HealthChecks UI docker image with https Dns.GetHostName()
                    options.AddHealthCheckEndpoint("Healthcheck API",
                        env.IsDevelopment() ? "/health" : $"http://{Dns.GetHostName()}/health");
                })
                .AddInMemoryStorage();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo() { Title = "Catalog.API", Version = "v1" });
            });
            services.AddAutoMapper(Assembly.GetAssembly(typeof(Catalog.Application.Mappers.ProductMapper)));
            services.AddMediatR(cgf => cgf.RegisterServicesFromAssemblies(Assembly.GetAssembly(typeof(CreateProductHandler))));
            services.AddScoped<ICatalogContext, CatalogContext>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ITypesRepository, ProductRepository>();
            services.AddScoped<IBrandRepository, ProductRepository>();
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog.API"));
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapHealthChecksUI(options => options.UIPath = "/dashboard");
                endpoints.MapControllers();
            });
        }
    }
}
