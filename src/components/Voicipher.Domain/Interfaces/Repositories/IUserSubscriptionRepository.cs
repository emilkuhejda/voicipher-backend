using System;
using System.Threading;
using System.Threading.Tasks;
using Voicipher.Domain.Models;

namespace Voicipher.Domain.Interfaces.Repositories
{
    public interface IUserSubscriptionRepository : IRepository<UserSubscription>
    {
        Task<TimeSpan> GetRemainingTimeAsync(Guid userId, CancellationToken cancellationToken);
    }
}
