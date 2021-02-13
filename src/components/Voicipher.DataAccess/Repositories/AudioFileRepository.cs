using System;
using System.Linq;
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

        public Task<AudioFile[]> GetAllAsync(Guid userId, DateTime updatedAfter, Guid applicationId)
        {
            return Context.AudioFiles
                .Where(x => !x.IsDeleted)
                .Where(x => x.UserId == userId && x.DateUpdatedUtc >= updatedAfter && x.ApplicationId != applicationId)
                .AsNoTracking()
                .ToArrayAsync();
        }
    }
}
