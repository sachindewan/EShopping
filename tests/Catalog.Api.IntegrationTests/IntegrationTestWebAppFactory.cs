using Catalog.API;
using Catalog.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using MongoDB.Driver;

namespace Catalog.Api.IntegrationTests
{
    public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
       
        public IntegrationTestWebAppFactory()
        {
        }
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                var descriptor = services
                    .SingleOrDefault(s => s.ServiceType == typeof(ICatalogContext));

                if (descriptor is not null)
                {
                    services.Remove(descriptor);
                }

                //services.AddScoped<ICatalogContext, CatalogContext>();
            });
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

       
        public new  Task DisposeAsync()
        {
            var client = new MongoClient("mongodb://127.0.0.1:27017?readPreference=primary&ssl=false");
            client.DropDatabase("ProductDb");
            return Task.CompletedTask;
        }
    }
}
