using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;

namespace Voicipher.DataAccess.Repositories
{
    public class BackgroundJobRepository : Repository<BackgroundJob>, IBackgroundJobRepository
    {
        public BackgroundJobRepository(DatabaseContext context)
            : base(context)
        {
        }

        public Task<BackgroundJob> GetJobForRestartAsync(Guid audioFileId, CancellationToken cancellationToken)
        {
            return Context.BackgroundJobs.FirstOrDefaultAsync(x => x.AudioFileId == audioFileId, cancellationToken);
        }

        public Task<BackgroundJob[]> GetJobsForRestartAsync(CancellationToken cancellationToken)
        {
            return Context.BackgroundJobs.Where(x => x.JobState < JobState.Completed).ToArrayAsync(cancellationToken);
        }

        public Task<int> GetAttemptsCountAsync(Guid audioFileId, CancellationToken cancellationToken)
        {
            return Context.BackgroundJobs
                .Where(x => x.AudioFileId == audioFileId)
                .Select(x => x.Attempt)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
