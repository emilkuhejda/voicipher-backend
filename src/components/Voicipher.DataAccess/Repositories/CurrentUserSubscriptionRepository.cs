using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;

namespace Voicipher.DataAccess.Repositories
{
    public class CurrentUserSubscriptionRepository : Repository<CurrentUserSubscription>, ICurrentUserSubscriptionRepository
    {
        public CurrentUserSubscriptionRepository(DatabaseContext context)
            : base(context)
        {
        }

        public Task<CurrentUserSubscription[]> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return Context.CurrentUserSubscriptions
                .Where(x => x.UserId == userId)
                .ToArrayAsync(cancellationToken);
        }

        public Task<DateTime> GetLastUpdateAsync(Guid userId, CancellationToken cancellationToken)
        {
            return Context.CurrentUserSubscriptions
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.DateUpdatedUtc)
                .Select(x => x.DateUpdatedUtc)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<TimeSpan> GetRemainingTimeAsync(Guid userId, CancellationToken cancellationToken)
        {
            var entity = await Context.CurrentUserSubscriptions.SingleOrDefaultAsync(x => x.UserId == userId, cancellationToken);
            if (entity == null)
                return TimeSpan.Zero;

            return entity.Time;
        }

        public void RemoveRange(CurrentUserSubscription[] currentUserSubscriptions)
        {
            Context.CurrentUserSubscriptions.RemoveRange(currentUserSubscriptions);
        }
    }
}
