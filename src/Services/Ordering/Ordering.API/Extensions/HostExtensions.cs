using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Ordering.Infrastructure.Persistence;

namespace Ordering.API.Extensions
{
    public static class HostExtensions
    {
        public static void MigrateDatabase<TContext>(
            this IHost host, 
            Action<TContext, IServiceProvider> seeder, 
            int? retry = 0) 
            where TContext : DbContext
        {
            int retryForAvailability = retry.Value;

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<TContext>>();
                var context = services.GetService<TContext>();

                try
                {
                    logger.LogInformation("Migrating database...");

                    InvokeSeeder(seeder, context, services);

                    logger.LogInformation("Migration completed.");
                }
                catch (SqlException ex)
                {
                    logger.LogError(ex, "An error ocurred in the migration.");
                    
                    if(retryForAvailability < 50)
                    {
                        retryForAvailability++;
                        System.Threading.Thread.Sleep(2000);
                        MigrateDatabase(host, seeder, retryForAvailability);
                    }
                }
            }
        }

        private static void InvokeSeeder<TContext>(
            Action<TContext, IServiceProvider> seeder, 
            TContext? context, 
            IServiceProvider services) 
            where TContext : DbContext
        {
            context?.Database.Migrate();
            seeder(context, services);
        }
    }
}
