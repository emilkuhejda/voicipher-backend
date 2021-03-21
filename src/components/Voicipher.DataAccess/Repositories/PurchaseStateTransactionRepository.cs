using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;

namespace Voicipher.DataAccess.Repositories
{
    public class PurchaseStateTransactionRepository : Repository<PurchaseStateTransaction>, IPurchaseStateTransactionRepository
    {
        public PurchaseStateTransactionRepository(DatabaseContext context)
            : base(context)
        {
        }
    }
}
