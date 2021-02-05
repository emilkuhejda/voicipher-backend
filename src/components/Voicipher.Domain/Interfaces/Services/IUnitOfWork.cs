using System.Threading;
using System.Threading.Tasks;

namespace Voicipher.Domain.Interfaces.Services
{
    public interface IUnitOfWork
    {
        Task SaveAsync(CancellationToken cancellationToken = default);
    }
}
