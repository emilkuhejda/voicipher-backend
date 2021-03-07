using System.Threading;
using System.Threading.Tasks;
using Voicipher.Domain.Models;

namespace Voicipher.Domain.Interfaces.Services
{
    public interface IWavFileService
    {
        Task<string> RunConversionToWavAsync(AudioFile audioFile, CancellationToken cancellationToken);

        Task<TranscribedAudioFile[]> SplitAudioFileAsync(AudioFile audioFile, CancellationToken cancellationToken);
    }
}
