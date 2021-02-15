using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;

namespace Voicipher.DataAccess.Repositories
{
    public class DeletedAccountRepository : Repository<DeletedAccount>, IDeletedAccountRepository
    {
        public DeletedAccountRepository(DatabaseContext context)
            : base(context)
        {
        }
    }
}
