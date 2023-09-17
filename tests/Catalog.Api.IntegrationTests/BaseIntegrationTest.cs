using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catalog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Api.IntegrationTests
{
    public class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>,
        IDisposable
    {
        private readonly IServiceScope _scope;
        protected readonly ISender Sender;

        protected readonly ICatalogContext CatalogContext;
        protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
        {
            _scope = factory.Services.CreateScope();

            Sender = _scope.ServiceProvider.GetRequiredService<ISender>();
            CatalogContext = _scope.ServiceProvider.GetRequiredService<ICatalogContext>();
        }
        public void Dispose()
        {
            _scope?.Dispose();
        }
    }
}
