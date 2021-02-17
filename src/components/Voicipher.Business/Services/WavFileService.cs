using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using Azure;
using NAudio.Wave;
using Serilog;
using Voicipher.DataAccess;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Models;

namespace Voicipher.Business.Services
{
    public class WavFileService : IWavFileService
    {
        private readonly IBlobStorage _blobStorage;
        private readonly IDiskStorage _diskStorage;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public WavFileService(
            IBlobStorage blobStorage,
            IIndex<StorageLocation, IDiskStorage> index,
            IUnitOfWork unitOfWork,
            ILogger logger)
        {
            _blobStorage = blobStorage;
            _diskStorage = index[StorageLocation.Audio];
            _unitOfWork = unitOfWork;
            _logger = logger.ForContext<WavFileService>();
        }

        public async Task RunConversionToWavAsync(AudioFile audioFile, CancellationToken cancellationToken)
        {
            _logger.Information($"Start conversion audio file {audioFile.Id} to wav format");

            var sourceFileNamePath = Path.Combine(_diskStorage.GetDirectoryPath(), audioFile.SourceFileName ?? string.Empty);
            if (File.Exists(sourceFileNamePath))
            {
                _logger.Information($"Source wav file is already exists in destination in destination {sourceFileNamePath}");
                return;
            }

            var tempFilePath = string.Empty;
            try
            {
                var getBlobSettings = new GetBlobSettings(audioFile.OriginalSourceFileName, audioFile.UserId, audioFile.Id);
                var bloBytes = await _blobStorage.GetAsync(getBlobSettings, cancellationToken);
                tempFilePath = await _diskStorage.UploadAsync(bloBytes, cancellationToken);
                var (wavFilePath, fileName) = await ConvertToWavAsync(tempFilePath);

                audioFile.SourceFileName = fileName;
                await _unitOfWork.SaveAsync(cancellationToken);

                _logger.Information($"Conversion audio file {audioFile.Id} to wav format finished. New audio file was stored in destination {wavFilePath}");
            }
            catch (RequestFailedException ex)
            {
                _logger.Error(ex,
                    $"Blob storage is unavailable. User ID = {audioFile.Id}, Audio files = {audioFile.Id}, file name = {audioFile.OriginalSourceFileName}");
                throw;
            }
            finally
            {
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }

        public async Task SplitAudioFile(AudioFile audioFile, BackgroundJob backgroundJob, CancellationToken cancellationToken)
        {
            var wavFilePath = Path.Combine(_diskStorage.GetDirectoryPath(), audioFile.SourceFileName ?? string.Empty);
            if (!File.Exists(wavFilePath))
                throw new FileNotFoundException($"Wav file {wavFilePath} does not exist");
        }

        public async Task<(string filePath, string fileName)> ConvertToWavAsync(string inputFilePath)
        {
            var fileName = $"{Guid.NewGuid()}.voc";
            var filePath = Path.Combine(_diskStorage.GetDirectoryPath(), fileName);
            await Task.Run(() =>
            {
                using (var reader = new MediaFoundationReader(inputFilePath))
                {
                    WaveFileWriter.CreateWaveFile(filePath, reader);
                }
            }).ConfigureAwait(false);

            _logger.Information($"File {inputFilePath} was converted and stored in new destination {filePath}");

            return (filePath, fileName);
        }
    }
}
