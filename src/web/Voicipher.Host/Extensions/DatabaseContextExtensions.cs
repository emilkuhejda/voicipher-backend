using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Voicipher.DataAccess;
using Voicipher.DataAccess.Extensions;

namespace Voicipher.Host.Extensions
{
    public static class DatabaseContextExtensions
    {
        public static void MigrateDatabase(this IApplicationBuilder app)
        {
            var serviceScopeFactory = app.ApplicationServices.GetService<IServiceScopeFactory>();
            if (serviceScopeFactory == null)
                return;

            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<DatabaseContext>();
                var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger>().ForContext<DatabaseContext>();

                try
                {
                    context.Database.Migrate();
                    context.SeedDatabase();
                }
                catch (Exception ex)
                {
                    logger.Fatal(ex, "Initializing of the database failed");
                }
            }
        }
    }
}
