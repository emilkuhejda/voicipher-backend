using System.Threading;
using System.Threading.Tasks;
using Voicipher.Domain.Models;

namespace Voicipher.Domain.Interfaces.Services
{
    public interface ISpeechRecognitionService
    {
        Task<TranscribeItem[]> RecognizeAsync(TranscribeAudioFile[] transcribeAudioFiles, string language, CancellationToken cancellationToken);
    }
}
