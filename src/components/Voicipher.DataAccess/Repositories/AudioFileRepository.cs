using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;

namespace Voicipher.DataAccess.Repositories
{
    public class AudioFileRepository : Repository<AudioFile>, IAudioFileRepository
    {
        public AudioFileRepository(DatabaseContext context)
            : base(context)
        {
        }

        public Task<AudioFile> GetAsync(Guid userId, Guid audioFileId, CancellationToken cancellationToken)
        {
            return Context.AudioFiles
                .Where(x => !x.IsDeleted)
                .FirstOrDefaultAsync(x => x.Id == audioFileId && x.UserId == userId, cancellationToken);
        }

        public Task<AudioFile[]> GetAllAsync(Guid userId, DateTime updatedAfter, Guid applicationId, CancellationToken cancellationToken)
        {
            return Context.AudioFiles
                .Where(x => !x.IsDeleted)
                .Where(x => x.UserId == userId && x.DateUpdatedUtc >= updatedAfter && x.ApplicationId != applicationId)
                .AsNoTracking()
                .ToArrayAsync(cancellationToken);
        }

        public Task<DateTime> GetLastUpdateAsync(Guid userId, CancellationToken cancellationToken)
        {
            return Context.AudioFiles
                .Where(x => !x.IsDeleted)
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.DateUpdatedUtc)
                .AsNoTracking()
                .Select(x => x.DateUpdatedUtc)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public Task<DateTime> GetDeletedLastUpdateAsync(Guid userId, CancellationToken cancellationToken)
        {
            return Context.AudioFiles
                .Where(x => x.IsDeleted)
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.DateUpdatedUtc)
                .AsNoTracking()
                .Select(x => x.DateUpdatedUtc)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public Task<Guid[]> GetAllDeletedIdsAsync(Guid userId, DateTime updatedAfter, Guid applicationId, CancellationToken cancellationToken)
        {
            return Context.AudioFiles
                .Where(x => x.IsDeleted)
                .Where(x => x.UserId == userId && x.DateUpdatedUtc >= updatedAfter && x.ApplicationId != applicationId)
                .AsNoTracking()
                .Select(x => x.Id)
                .ToArrayAsync(cancellationToken);
        }

        public Task<AudioFile[]> GetTemporaryDeletedFileItemsAsync(Guid userId, CancellationToken cancellationToken)
        {
            return Context.AudioFiles
                .Where(x => x.UserId == userId)
                .Where(x => x.IsDeleted && !x.IsPermanentlyDeleted)
                .AsNoTracking()
                .ToArrayAsync(cancellationToken);
        }

        public async Task DeleteAllAsync(Guid userId, DeletedAudioFile[] audioFiles, Guid applicationId, CancellationToken cancellationToken)
        {
            var fileItemIds = audioFiles.Select(x => x.Id);
            var entities = await Context.AudioFiles
                .Where(x => !x.IsDeleted)
                .Where(x => fileItemIds.Contains(x.Id) && x.UserId == userId)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            if (!entities.Any())
                return;

            foreach (var entity in entities)
            {
                var deletedFileItem = audioFiles.Single(x => x.Id == entity.Id);
                if (deletedFileItem.DeletedDate < entity.DateUpdatedUtc)
                    continue;

                entity.ApplicationId = applicationId;
                entity.DateUpdatedUtc = DateTime.UtcNow;
                entity.IsDeleted = true;
            }
        }
    }
}
