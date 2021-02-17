using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using Azure;
using Microsoft.Extensions.Options;
using Serilog;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Interfaces.Commands.Transcription;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Models;
using Voicipher.Domain.Settings;

namespace Voicipher.Business.Services
{
    public class WavFileService : IWavFileService
    {
        private readonly IBlobStorage _blobStorage;
        private readonly IDiskStorage _diskStorage;
        private readonly AppSettings _appSettings;
        private readonly ILogger _logger;

        public WavFileService(
            IBlobStorage blobStorage,
            IIndex<StorageLocation, IDiskStorage> index,
            IOptions<AppSettings> options,
            ILogger logger)
        {
            _blobStorage = blobStorage;
            _diskStorage = index[StorageLocation.Chunk];
            _appSettings = options.Value;
            _logger = logger.ForContext<WavFileService>();
        }

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
