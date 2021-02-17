using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Serilog;
using Voicipher.Domain.Interfaces.Repositories;

namespace Voicipher.Business.BackgroundServices
{
    public class BackgroundJobStarterService : BackgroundService
    {
        private readonly IBackgroundJobRepository _backgroundJobRepository;
        private readonly ILogger _logger;

        public BackgroundJobStarterService(IBackgroundJobRepository backgroundJobRepository, ILogger logger)
        {
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
        }
    }
}
