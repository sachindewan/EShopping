using System.Reflection;
using Basket.Application.Handlers;
using Basket.Core.Repositories;
using Basket.Infrastructure.Repositories;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace Basket.API
{
    public class Startup
    {
        public IConfiguration Configuration;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApiVersioning(options =>
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
            });

            services.AddStackExchangeRedisCache(cacheOptions =>
            {
                cacheOptions.Configuration = Configuration.GetValue<string>("CacheSettings:ConnectionString");
            });
            services.AddMediatR(src=>src.RegisterServicesFromAssembly(typeof(CreateShoppingCartCommandHandler).GetTypeInfo().Assembly));
            services.AddScoped<IBasketRepository, BasketRepository>();
            services.AddAutoMapper(typeof(Startup));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo() { Title = "Basket.API", Version = "v1" });
            });
            services.AddHealthChecks()
                .AddRedis(Configuration["CacheSettings:ConnectionString"], "Redis Health", HealthStatus.Degraded);
            services.AddControllers();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Basket.API"));

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health", new HealthCheckOptions
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
            });
        }
    }
}
