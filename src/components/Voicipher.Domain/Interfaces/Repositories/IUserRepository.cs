using System;
using System.Threading;
using System.Threading.Tasks;
using Voicipher.Domain.Models;

namespace Voicipher.Domain.Interfaces.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByEmailAsync(Guid userId, string email, CancellationToken cancellationToken);

        Task<bool> ExistsAsync(Guid userId, CancellationToken cancellationToken);
    }
}
