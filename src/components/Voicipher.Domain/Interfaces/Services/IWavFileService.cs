using System.Threading;
using System.Threading.Tasks;
using Voicipher.Domain.Models;

namespace Voicipher.Domain.Interfaces.Services
{
    public interface IWavFileService
    {
        Task RunConversionToWavAsync(AudioFile audioFile, CancellationToken cancellationToken);

        Task SplitAudioFile(AudioFile audioFile, BackgroundJob backgroundJob, CancellationToken cancellationToken);
    }
}
