using System;
using System.Threading;
using System.Threading.Tasks;
using Voicipher.Domain.Models;

namespace Voicipher.Domain.Interfaces.Repositories
{
    public interface ICurrentUserSubscriptionRepository : IRepository<CurrentUserSubscription>
    {
        Task<CurrentUserSubscription> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);

        Task<DateTime> GetLastUpdateAsync(Guid userId, CancellationToken cancellationToken);

        Task<TimeSpan> GetRemainingTimeAsync(Guid userId, CancellationToken cancellationToken);
    }
}
