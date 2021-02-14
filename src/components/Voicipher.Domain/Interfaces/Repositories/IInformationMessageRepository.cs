using System;
using System.Threading;
using System.Threading.Tasks;
using Voicipher.Domain.Models;

namespace Voicipher.Domain.Interfaces.Repositories
{
    public interface IInformationMessageRepository : IRepository<InformationMessage>
    {
        Task<InformationMessage> GetByIdAsync(Guid informationMessageId, CancellationToken cancellationToken);

        Task<InformationMessage[]> GetAllAsync(Guid userId, DateTime updatedAfter, CancellationToken cancellationToken);

        Task<InformationMessage[]> GetAllShallowAsync(CancellationToken cancellationToken);

        Task<DateTime> GetLastUpdateAsync(Guid userId, CancellationToken cancellationToken);
    }
}
