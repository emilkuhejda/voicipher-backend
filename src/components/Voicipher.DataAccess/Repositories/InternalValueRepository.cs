using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;

namespace Voicipher.DataAccess.Repositories
{
    public class InternalValueRepository : Repository<InternalValue>, IInternalValueRepository
    {
        public InternalValueRepository(DatabaseContext context)
            : base(context)
        {
        }
    }
}
