using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;

namespace Voicipher.DataAccess.Repositories
{
    public class RecognizedAudioSampleRepository : Repository<RecognizedAudioSample>, IRecognizedAudioSampleRepository
    {
        public RecognizedAudioSampleRepository(DatabaseContext context)
            : base(context)
        {
        }
    }
}
