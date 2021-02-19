using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;

namespace Voicipher.DataAccess.Repositories
{
    public class BillingPurchaseRepository : Repository<BillingPurchase>, IBillingPurchaseRepository
    {
        public BillingPurchaseRepository(DatabaseContext context)
            : base(context)
        {
        }

        public Task<BillingPurchase[]> GetAllAsync(Guid userId, CancellationToken cancellationToken)
        {
            return Context.BillingPurchases
                .Where(x => x.UserId == userId)
                .AsNoTracking()
                .ToArrayAsync(cancellationToken);
        }
    }
}
