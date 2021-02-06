using System.Threading;
using System.Threading.Tasks;
using Voicipher.Domain.Models;

namespace Voicipher.Domain.Interfaces.Repositories
{
    public interface IRepository<T> where T : EntityBase
    {
        Task AddAsync(T entity);

        T Update(T entity);

        void Remove(T entity);

        Task<T[]> GetAllAsync(CancellationToken cancellationToken);

        Task SaveAsync(CancellationToken cancellationToken);
    }
}
