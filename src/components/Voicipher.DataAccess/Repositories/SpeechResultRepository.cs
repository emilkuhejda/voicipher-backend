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

        public void UpdateAll(SpeechResult[] speechResults)
        {
            foreach (var speechResult in speechResults)
            {
                Context.Attach(speechResult);
                Context.Entry(speechResult).Property(x => x.TotalTime).IsModified = true;
            }
        }
    }
}
