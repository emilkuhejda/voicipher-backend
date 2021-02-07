using System.Threading;
using System.Threading.Tasks;
using Voicipher.Domain.Models;

namespace Voicipher.Domain.Interfaces.Repositories
{
    public interface IUserDeviceRepository : IRepository<UserDevice>
    {
        Task AddOrUpdateAsync(UserDevice userDevice, CancellationToken cancellationToken);
    }
}
