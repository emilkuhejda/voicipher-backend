using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Interfaces.Services;

namespace Voicipher.Business.BackgroundServices
{
    public class BackgroundJobStarterService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IBackgroundJobRepository _backgroundJobRepository;
        private readonly ILogger _logger;

        public BackgroundJobStarterService(
            IServiceProvider serviceProvider,
            IBackgroundJobRepository backgroundJobRepository,
            ILogger logger)
        {
            _serviceProvider = serviceProvider;
            _backgroundJobRepository = backgroundJobRepository;
            _logger = logger.ForContext<BackgroundJobStarterService>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();

            var jobs = await _backgroundJobRepository.GetJobsForRestartAsync(stoppingToken);
            if (jobs.Any())
            {
                _logger.Information($"{jobs.Length} background jobs for restart");
            }

            using (var scope = _serviceProvider.CreateScope())
            {
                var diskStorage = scope.ServiceProvider.GetRequiredService<IIndex<StorageLocation, IDiskStorage>>()[StorageLocation.Audio];
                diskStorage.RemoveTemporaryFolder();
            }
        }
    }
}
