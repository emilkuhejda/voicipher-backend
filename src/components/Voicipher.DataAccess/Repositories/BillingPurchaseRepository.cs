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
    }
}
