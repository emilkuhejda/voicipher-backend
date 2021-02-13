using System;
using System.Threading.Tasks;
using Voicipher.Domain.Models;

namespace Voicipher.Domain.Interfaces.Repositories
{
    public interface IFileChunkRepository : IRepository<FileChunk>
    {
        Task<FileChunk[]> GetByAudioFileIdAsync(Guid audioFileId);

        void DeleteByAudioFileId(FileChunk[] fileChunks);
    }
}
