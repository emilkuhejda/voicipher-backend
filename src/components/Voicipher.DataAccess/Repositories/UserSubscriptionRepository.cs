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

        public Task<UserSubscription[]> GetAllAsync(Guid userId, CancellationToken cancellationToken)
        {
            return Context.UserSubscriptions
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .ToArrayAsync(cancellationToken);
        }
    }
}
