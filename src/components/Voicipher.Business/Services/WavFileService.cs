using System.Threading;
using System.Threading.Tasks;
using Azure;
using Serilog;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Models;

namespace Voicipher.Business.Services
{
    public class WavFileService : IWavFileService
    {
        private readonly IBlobStorage _blobStorage;
        private readonly ILogger _logger;

        public async Task RunConversionToWavAsync(AudioFile audioFile, CancellationToken cancellationToken)
        {
            _logger.Information($"Start conversion audio file {audioFile.Id} to wav format");

            try
            {
                var getBlobSettings = new GetBlobSettings(audioFile.OriginalSourceFileName, audioFile.UserId, audioFile.Id);
                var bloBytes = await _blobStorage.GetAsync(getBlobSettings, cancellationToken);
            }
            catch (RequestFailedException ex)
            {
                _logger.Error(ex, $"Blob storage is unavailable. User ID = {audioFile.Id}, Audio files = {audioFile.Id}, file name = {audioFile.OriginalSourceFileName}");
                throw;
            }
        }
    }
}
