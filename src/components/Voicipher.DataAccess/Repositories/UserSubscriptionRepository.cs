using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;

namespace Voicipher.DataAccess.Repositories
{
    public class UserSubscriptionRepository : Repository<UserSubscription>, IUserSubscriptionRepository
    {
        public UserSubscriptionRepository(DatabaseContext context)
            : base(context)
        {
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
    }
}
