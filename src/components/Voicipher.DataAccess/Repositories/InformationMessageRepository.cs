using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;

namespace Voicipher.DataAccess.Repositories
{
    public class InformationMessageRepository : Repository<InformationMessage>, IInformationMessageRepository
    {
        public InformationMessageRepository(DatabaseContext context)
            : base(context)
        {
        }

        public Task<InformationMessage> GetByIdAsync(Guid informationMessageId, CancellationToken cancellationToken)
        {
            return Context.InformationMessages
                .Where(x => x.Id == informationMessageId)
                .Include(x => x.LanguageVersions)
                .AsNoTracking()
                .SingleOrDefaultAsync(cancellationToken);
        }

        public Task<InformationMessage[]> GetByUserIdAsync(Guid userId, Guid[] ids, CancellationToken cancellationToken)
        {
            return Context.InformationMessages
                .Where(x => ids.Contains(x.Id) && x.UserId == userId)
                .ToArrayAsync(cancellationToken);
        }

        public Task<InformationMessage[]> GetAllAsync(Guid userId, DateTime updatedAfter, CancellationToken cancellationToken)
        {
            return Context.InformationMessages
                .Include(x => x.LanguageVersions)
                .AsNoTracking()
                .OrderByDescending(x => x.DateUpdatedUtc)
                .Where(x => (!x.UserId.HasValue || x.UserId.Value == userId) &&
                            ((x.DatePublishedUtc.HasValue && x.DatePublishedUtc >= updatedAfter) ||
                             (x.DateUpdatedUtc.HasValue && x.DateUpdatedUtc >= updatedAfter)))
                .ToArrayAsync(cancellationToken);
        }

        public Task<InformationMessage[]> GetAllShallowAsync(CancellationToken cancellationToken)
        {
            return Context.InformationMessages
                .Where(x => !x.UserId.HasValue)
                .AsNoTracking()
                .ToArrayAsync(cancellationToken);
        }

        public async Task<DateTime> GetLastUpdateAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await Context.InformationMessages
                .Where(x => (!x.UserId.HasValue || x.UserId.Value == userId) &&
                            (x.DatePublishedUtc.HasValue || x.DateUpdatedUtc.HasValue))
                .OrderByDescending(x => x.DateUpdatedUtc)
                .Select(x => x.DateUpdatedUtc)
                .FirstOrDefaultAsync(cancellationToken) ?? DateTime.MinValue;
        }
    }
}
