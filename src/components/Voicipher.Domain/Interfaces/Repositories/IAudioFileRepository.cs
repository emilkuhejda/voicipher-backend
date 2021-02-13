using System;
using System.Threading.Tasks;
using Voicipher.Domain.Models;

namespace Voicipher.Domain.Interfaces.Repositories
{
    public interface IAudioFileRepository : IRepository<AudioFile>
    {
        Task<AudioFile[]> GetAllAsync(Guid userId, DateTime updatedAfter, Guid applicationId);
    }
}
