using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;

namespace Voicipher.DataAccess.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(DatabaseContext context)
            : base(context)
        {
        }

        public Task<User> GetByEmailAsync(Guid userId, string email, CancellationToken cancellationToken)
        {
            return Context.Users.SingleOrDefaultAsync(x => x.Id == userId && x.Email == email, cancellationToken);
        }
    }
}
