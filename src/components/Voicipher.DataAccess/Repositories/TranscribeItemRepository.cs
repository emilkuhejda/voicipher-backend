using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;

namespace Voicipher.DataAccess.Repositories
{
    public class TranscribeItemRepository : Repository<TranscribeItem>, ITranscribeItemRepository
    {
        public TranscribeItemRepository(DatabaseContext context)
            : base(context)
        {
        }
    }
}
