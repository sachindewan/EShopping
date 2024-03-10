using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
namespace Order.Api.Extensions
{
    public static class DbExtension
    {
        public static IHost MigrateDatabase<TContext>(this IHost host, Action<TContext, IServiceProvider> seeder)
        where TContext : DbContext
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<TContext>>();
                var context = services.GetService<TContext>();

                try
                {
                    logger.LogInformation($"Started Db Migration: {typeof(TContext).Name}");

                    CallSeeder(seeder, context, services);
                    logger.LogInformation($"Migration Completed: {typeof(TContext).Name}");
                }
                catch (SqlException e)
                {
                    logger.LogError(e, $"An error occurred while migrating db: {typeof(TContext).Name}");
                }
            }

            return host;
        }

        private static void CallSeeder<TContext>(Action<TContext, IServiceProvider> seeder, TContext context,
            IServiceProvider services) where TContext : DbContext
        {

            if (context.Database.GetPendingMigrationsAsync().GetAwaiter().GetResult().Count() > 0)
            {
                context.Database.Migrate();
            }
            seeder(context, services);
        }
    }
}
