using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Catalog.API.HealthCheck
{
    public class MongoHealthCheck : IHealthCheck
    {
        private IMongoDatabase _db { get; set; }
        public MongoClient _mongoClient { get; set; }

        public MongoHealthCheck(IConfiguration configuration)
        {
            _mongoClient = new MongoClient(configuration["DatabaseSettings:ConnectionString"]);

            _db = _mongoClient.GetDatabase(configuration["DatabaseSettings:DatabaseName"]);

        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            var healthCheckResultHealthy = await CheckMongoDBConnectionAsync();


            if (healthCheckResultHealthy)
            {
                return HealthCheckResult.Healthy("MongoDB health check success");
            }

            return HealthCheckResult.Unhealthy("MongoDB health check failure"); ;
        }

        private async Task<bool> CheckMongoDBConnectionAsync()
        {
            try
            {
                await _db.RunCommandAsync((Command<BsonDocument>)"{ping:1}");
            }

            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
