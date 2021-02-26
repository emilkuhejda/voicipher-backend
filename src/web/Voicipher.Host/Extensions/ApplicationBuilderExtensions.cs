using System;
using Autofac.Features.Indexed;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Voicipher.DataAccess;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Models;

namespace Voicipher.Host.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void CleanTemporaryData(this IApplicationBuilder app)
        {
            var serviceScopeFactory = app.ApplicationServices.GetService<IServiceScopeFactory>();
            if (serviceScopeFactory == null)
                return;

            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<DatabaseContext>();
                var index = serviceScope.ServiceProvider.GetRequiredService<IIndex<StorageLocation, IDiskStorage>>();
                var chunkStorage = index[StorageLocation.Chunk];
                var audioStorage = index[StorageLocation.Audio];
                var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger>().ForContext<Startup>();

                try
                {
                    logger.Information("Start cleaning temporary data");

                    context.Database.ExecuteSqlRaw($"TRUNCATE TABLE [{nameof(FileChunk)}]");

                    chunkStorage.RemoveTemporaryFolder();
                    audioStorage.RemoveTemporaryFolder();

                    logger.Information("Finish of the cleaning temporary data");
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Cleaning temporary data failed");
                }
            }
        }
    }
}
