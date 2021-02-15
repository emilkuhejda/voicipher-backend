using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;

namespace Voicipher.DataAccess.Repositories
{
    public class SpeechResultRepository : Repository<SpeechResult>, ISpeechResultRepository
    {
        public SpeechResultRepository(DatabaseContext context)
            : base(context)
        {
        }
    }
}
