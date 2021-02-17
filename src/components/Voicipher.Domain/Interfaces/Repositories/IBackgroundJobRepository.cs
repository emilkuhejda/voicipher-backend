using System.Threading;
using System.Threading.Tasks;
using Voicipher.Domain.Models;

namespace Voicipher.Domain.Interfaces.Repositories
{
    public interface IBackgroundJobRepository : IRepository<BackgroundJob>
    {
        Task<BackgroundJob[]> GetJobsForRestartAsync(CancellationToken cancellationToken);
    }
}
