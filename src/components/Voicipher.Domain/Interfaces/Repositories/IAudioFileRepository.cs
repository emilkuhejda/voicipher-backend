using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Voicipher.Domain.Models;

namespace Voicipher.Domain.Interfaces.Repositories
{
    public interface IAudioFileRepository : IRepository<AudioFile>
    {
        Task<AudioFile> GetAsync(Guid userId, Guid audioFileId, CancellationToken cancellationToken);

        Task<AudioFile[]> GetAllAsync(Guid userId, DateTime updatedAfter, Guid applicationId, CancellationToken cancellationToken);

        Task<AudioFile[]> GetAllCreatedAsync(Guid userId, CancellationToken cancellationToken);

        Task<DateTime> GetLastUpdateAsync(Guid userId, CancellationToken cancellationToken);

        Task<DateTime> GetDeletedLastUpdateAsync(Guid userId, CancellationToken cancellationToken);

        Task<Guid[]> GetAllDeletedIdsAsync(Guid userId, DateTime updatedAfter, Guid applicationId, CancellationToken cancellationToken);

        Task<AudioFile[]> GetTemporaryDeletedAudioFilesAsync(Guid userId, CancellationToken cancellationToken);

        Task<AudioFile[]> GetForDeleteAllAsync(Guid userId, Guid[] audioFileIds, Guid applicationId, CancellationToken cancellationToken);

        Task<AudioFile[]> GetForPermanentDeleteAllAsync(Guid userId, IEnumerable<Guid> fileItemIds, Guid applicationId, CancellationToken cancellationToken);

        Task<AudioFile[]> GetForRestoreAsync(Guid userId, Guid[] audioFileIds, Guid applicationId, CancellationToken cancellationToken);

        Task<AudioFile> GetWithTranscribeItemsAsync(Guid userId, Guid audioFileId, CancellationToken cancellationToken);

        Task<AudioFile[]> GetInProgressAsync(CancellationToken cancellationToken);

        Task<AudioFile[]> GetAllForPermanentDeleteAsync(DateTime deleteBefore, CancellationToken cancellationToken);

        Task<AudioFile[]> GetAllForCleanUpAsync(DateTime deleteBefore, CancellationToken cancellationToken);
    }
}
