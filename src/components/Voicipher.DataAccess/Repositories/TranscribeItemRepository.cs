using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;

namespace Voicipher.DataAccess.Repositories
{
    public class TranscribeItemRepository : Repository<TranscribeItem>, ITranscribeItemRepository
    {
        public TranscribeItemRepository(DatabaseContext context)
            : base(context)
        {
        }

        public Task<TranscribeItem[]> GetAllAfterDateAsync(Guid userId, DateTime updatedAfter, Guid applicationId, CancellationToken cancellationToken)
        {
            return Context.TranscribeItems
                .Where(x => x.AudioFile.UserId == userId && x.DateUpdatedUtc >= updatedAfter && x.ApplicationId != applicationId)
                .AsNoTracking()
                .OrderBy(x => x.StartTime)
                .ToArrayAsync(cancellationToken);
        }
    }
}
