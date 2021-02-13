using System;
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

        public async Task<TimeSpan> GetRemainingTimeAsync(Guid userId, CancellationToken cancellationToken)
        {
            var entity = await Context.CurrentUserSubscriptions.SingleOrDefaultAsync(x => x.UserId == userId, cancellationToken);
            if (entity == null)
                return TimeSpan.Zero;

            return entity.Time;
        }
    }
}
