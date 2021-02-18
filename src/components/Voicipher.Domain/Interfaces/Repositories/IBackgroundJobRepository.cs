using System;
using System.Threading;
using System.Threading.Tasks;
using Voicipher.Domain.Models;

namespace Voicipher.Domain.Interfaces.Repositories
{
    public interface IBackgroundJobRepository : IRepository<BackgroundJob>
    {
        Task<BackgroundJob> GetJobForRestartAsync(Guid audioFileId, CancellationToken cancellationToken);

        Task<BackgroundJob[]> GetJobsForRestartAsync(CancellationToken cancellationToken);

        Task<int> GetAttemptsCountAsync(Guid audioFileId, CancellationToken cancellationToken);
    }
}
