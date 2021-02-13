using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;

namespace Voicipher.DataAccess.Repositories
{
    public class FileChunkRepository : Repository<FileChunk>, IFileChunkRepository
    {
        public FileChunkRepository(DatabaseContext context)
            : base(context)
        {
        }

        public Task<FileChunk[]> GetByAudioFileIdAsync(Guid audioFileId)
        {
            return Context.FileChunks.Where(x => x.AudioFileId == audioFileId).ToArrayAsync();
        }

        public void RemoveRange(FileChunk[] fileChunks)
        {
            Context.FileChunks.RemoveRange(fileChunks);
        }
    }
}
