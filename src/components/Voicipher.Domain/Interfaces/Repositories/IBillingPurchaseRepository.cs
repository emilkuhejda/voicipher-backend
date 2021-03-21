using System;
using System.Threading;
using System.Threading.Tasks;
using Voicipher.Domain.Models;

namespace Voicipher.Domain.Interfaces.Repositories
{
    public interface IBillingPurchaseRepository : IRepository<BillingPurchase>
    {
        Task<BillingPurchase> GetByIdAsync(Guid billingPurchaseId, CancellationToken cancellationToken);

        Task<BillingPurchase> GetByPurchaseIdAsync(string purchaseId, CancellationToken cancellationToken);

        Task<BillingPurchase[]> GetAllAsync(Guid userId, CancellationToken cancellationToken);
    }
}
