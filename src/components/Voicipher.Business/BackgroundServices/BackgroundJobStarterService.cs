using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Voicipher.Business.BackgroundServices
{
    public class BackgroundJobStarterService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;

        public BackgroundJobStarterService(
            IServiceProvider serviceProvider,
            ILogger logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger.ForContext<BackgroundJobStarterService>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();

            //using (var scope = _serviceProvider.CreateScope())
            //{
            //    var backgroundJobRepository = scope.ServiceProvider.GetRequiredService<IBackgroundJobRepository>();
            //    var backgroundJobs = await backgroundJobRepository.GetJobsForRestartAsync(stoppingToken);
            //    if (backgroundJobs.Any())
            //    {
            //        _logger.Information($"{backgroundJobs.Length} background jobs for restart");

            //        foreach (var backgroundJob in backgroundJobs)
            //        {
            //            try
            //            {
            //                var runBackgroundJobCommand = scope.ServiceProvider.GetRequiredService<IRunBackgroundJobCommand>();
            //                var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
            //                var backgroundJobPayload = mapper.Map<BackgroundJobPayload>(backgroundJob);
            //                await runBackgroundJobCommand.ExecuteAsync(backgroundJobPayload, null, stoppingToken);
            //            }
            //            catch (Exception ex)
            //            {
            //                _logger.Fatal(ex, "Background job failed");
            //            }
            //        }
            //    }
            //}

            //using (var scope = _serviceProvider.CreateScope())
            //{
            //    var diskStorage = scope.ServiceProvider.GetRequiredService<IIndex<StorageLocation, IDiskStorage>>()[StorageLocation.Audio];
            //    diskStorage.RemoveTemporaryFolder();
            //}
        }
    }
}
