using System;
using System.Threading;
using System.Threading.Tasks;
using Voicipher.Domain.Models;

namespace Voicipher.Domain.Interfaces.Repositories
{
    public interface ITranscribeItemRepository : IRepository<TranscribeItem>
    {
        Task<TranscribeItem[]> GetAllAfterDateAsync(Guid userId, DateTime updatedAfter, Guid applicationId, CancellationToken cancellationToken);

        Task<DateTime> GetLastUpdateAsync(Guid userId, CancellationToken cancellationToken);
    }
}
