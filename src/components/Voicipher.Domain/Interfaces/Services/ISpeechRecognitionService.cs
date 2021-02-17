using System.Threading.Tasks;

namespace Voicipher.Domain.Interfaces.Services
{
    public interface ISpeechRecognitionService
    {
        Task RecognizeAsync();
    }
}
