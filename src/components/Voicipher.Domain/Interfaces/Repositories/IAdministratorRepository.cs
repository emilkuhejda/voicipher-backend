using System.Threading;
using System.Threading.Tasks;
using Voicipher.Domain.Models;

namespace Voicipher.Domain.Interfaces.Repositories
{
    public interface IAdministratorRepository : IRepository<Administrator>
    {
        Task<Administrator> GetByNameAsync(string username, CancellationToken cancellationToken);
    }
}
