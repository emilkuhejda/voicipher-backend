﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Voicipher.Domain.Models;

namespace Voicipher.Domain.Interfaces.Repositories
{
    public interface IAudioFileRepository : IRepository<AudioFile>
    {
        Task<AudioFile> GetAsync(Guid userId, Guid fileItemId, CancellationToken cancellationToken);

        Task<AudioFile[]> GetAllAsync(Guid userId, DateTime updatedAfter, Guid applicationId, CancellationToken cancellationToken);

        Task<DateTime> GetLastUpdateAsync(Guid userId, CancellationToken cancellationToken);

        Task<DateTime> GetDeletedLastUpdateAsync(Guid userId, CancellationToken cancellationToken);

        Task<Guid[]> GetAllDeletedIdsAsync(Guid userId, DateTime updatedAfter, Guid applicationId, CancellationToken cancellationToken);

        Task<AudioFile[]> GetTemporaryDeletedFileItemsAsync(Guid userId, CancellationToken cancellationToken);
    }
}
