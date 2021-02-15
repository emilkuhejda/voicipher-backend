using Voicipher.Domain.Models;

namespace Voicipher.Domain.Interfaces.Repositories
{
    public interface ISpeechResultRepository : IRepository<SpeechResult>
    {
        void UpdateAll(SpeechResult[] speechResults);
    }
}
