using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Payloads.Job;

namespace Voicipher.Business.BackgroundServices
{
    public class BackgroundJobStarterService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public BackgroundJobStarterService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();

            using (var scope = _serviceProvider.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger>().ForContext<BackgroundJobStarterService>();
                var backgroundJobRepository = scope.ServiceProvider.GetRequiredService<IBackgroundJobRepository>();
                var backgroundJobs = await backgroundJobRepository.GetJobsForRestartAsync(stoppingToken);
                if (backgroundJobs.Any())
                {
                    logger.Information($"{backgroundJobs.Length} background jobs for restart");

                    foreach (var backgroundJob in backgroundJobs)
                    {
                        try
                        {
                            var runBackgroundJobCommand = scope.ServiceProvider.GetRequiredService<IRunBackgroundJobCommand>();
                            var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
                            var backgroundJobPayload = mapper.Map<BackgroundJobPayload>(backgroundJob);
                            await runBackgroundJobCommand.ExecuteAsync(backgroundJobPayload, null, stoppingToken);
                        }
                        catch (Exception ex)
                        {
                            logger.Fatal(ex, "Background job failed");
                        }
                    }
                }
            }

            using (var scope = _serviceProvider.CreateScope())
            {
                var diskStorage = scope.ServiceProvider.GetRequiredService<IIndex<StorageLocation, IDiskStorage>>()[StorageLocation.Audio];
                diskStorage.RemoveTemporaryFolder();
            }
        }
    }
}
