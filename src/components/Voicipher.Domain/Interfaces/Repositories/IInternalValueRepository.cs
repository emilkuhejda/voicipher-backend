using System.Threading;
using System.Threading.Tasks;
using Voicipher.Domain.Models;

namespace Voicipher.Domain.Interfaces.Repositories
{
    public interface IInternalValueRepository : IRepository<InternalValue>
    {
        Task<InternalValue> GetValueAsync(string key, CancellationToken cancellationToken);
    }
}
