using Catalog.Application.Queries;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catalog.Core.Specs;

namespace Catalog.Api.IntegrationTests
{
    public class CatalogTests : BaseIntegrationTest
    {
        public CatalogTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }
        [Fact]
        public async Task GetCatalog_ShouldReturnCatalog()
        {
            // Arrange
            var command = new GetAllProductsQuery(
                 new CatalogSpecParams()
                {
                    PageIndex = 1,
                    PageSize = 10
                });
            
        // Act
            var productResponse = await Sender.Send(command);

           

            Assert.NotNull(productResponse.Data);
        }
    }
}
