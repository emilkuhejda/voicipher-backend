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
    }
}
