using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Voicipher.Domain.Enums;
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

        public Task<AudioFile[]> GetAllCreatedAsync(Guid userId, CancellationToken cancellationToken)
        {
            return Context.AudioFiles
                .Where(x => !x.IsPermanentlyDeleted)
                .Where(x => x.UserId == userId)
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

        public Task<AudioFile[]> GetTemporaryDeletedAudioFilesAsync(Guid userId, CancellationToken cancellationToken)
        {
            return Context.AudioFiles
                .Where(x => x.UserId == userId)
                .Where(x => x.IsDeleted && !x.IsPermanentlyDeleted)
                .AsNoTracking()
                .ToArrayAsync(cancellationToken);
        }

        public Task<AudioFile[]> GetForDeleteAllAsync(Guid userId, Guid[] audioFileIds, Guid applicationId, CancellationToken cancellationToken)
        {
            return Context.AudioFiles
                .Where(x => !x.IsDeleted)
                .Where(x => audioFileIds.Contains(x.Id) && x.UserId == userId)
                .ToArrayAsync(cancellationToken);
        }

        public Task<AudioFile[]> GetForPermanentDeleteAllAsync(Guid userId, IEnumerable<Guid> fileItemIds, Guid applicationId, CancellationToken cancellationToken)
        {
            return Context.AudioFiles
                .Where(x => fileItemIds.Contains(x.Id) && x.UserId == userId)
                .AsNoTracking()
                .ToArrayAsync(cancellationToken);
        }

        public Task<AudioFile[]> GetForRestoreAsync(Guid userId, Guid[] audioFileIds, Guid applicationId, CancellationToken cancellationToken)
        {
            return Context.AudioFiles
                .Where(x => audioFileIds.Contains(x.Id) && x.UserId == userId)
                .ToArrayAsync(cancellationToken);
        }

        public Task<AudioFile> GetWithTranscribeItemsAsync(Guid userId, Guid audioFileId, CancellationToken cancellationToken)
        {
            return Context.AudioFiles
                .Where(x => !x.IsDeleted)
                .Include(x => x.TranscribeItems)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == audioFileId && x.UserId == userId, cancellationToken);
        }

        public Task<AudioFile[]> GetInProgressAsync(CancellationToken cancellationToken)
        {
            return Context.AudioFiles
                .Where(x => !x.IsDeleted)
                .Where(x => x.RecognitionState == RecognitionState.InProgress)
                .ToArrayAsync(cancellationToken);
        }

        public Task<AudioFile[]> GetAllForCleanUpAsync(CancellationToken cancellationToken)
        {
            return Context.AudioFiles
                .Include(x => x.TranscribeItems)
                .AsNoTracking()
                .Where(x => x.IsDeleted && !x.IsPermanentlyDeleted)
                .ToArrayAsync(cancellationToken);
        }
    }
}
