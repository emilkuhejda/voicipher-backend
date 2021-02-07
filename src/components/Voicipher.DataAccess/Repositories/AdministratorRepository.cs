using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;

namespace Voicipher.DataAccess.Repositories
{
    public class AdministratorRepository : Repository<Administrator>, IAdministratorRepository
    {
        public AdministratorRepository(DatabaseContext context)
            : base(context)
        {
        }

        public Task<Administrator> GetByNameAsync(string username, CancellationToken cancellationToken)
        {
            return Context.Administrators.SingleOrDefaultAsync(x => x.Username == username, cancellationToken);
        }
    }
}
