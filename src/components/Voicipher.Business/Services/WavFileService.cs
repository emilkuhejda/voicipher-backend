using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using Azure;
using NAudio.Wave;
using Serilog;
using Voicipher.DataAccess;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Models;

namespace Voicipher.Business.Services
{
    public class WavFileService : IWavFileService
    {
        private const int FileLengthInSeconds = 59;

        private readonly IBlobStorage _blobStorage;
        private readonly IDiskStorage _diskStorage;
        private readonly ICurrentUserSubscriptionRepository _currentUserSubscriptionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public WavFileService(
            IBlobStorage blobStorage,
            IIndex<StorageLocation, IDiskStorage> index,
            ICurrentUserSubscriptionRepository currentUserSubscriptionRepository,
            IUnitOfWork unitOfWork,
            ILogger logger)
        {
            _blobStorage = blobStorage;
            _diskStorage = index[StorageLocation.Audio];
            _currentUserSubscriptionRepository = currentUserSubscriptionRepository;
            _unitOfWork = unitOfWork;
            _logger = logger.ForContext<WavFileService>();
        }

        public async Task RunConversionToWavAsync(AudioFile audioFile, CancellationToken cancellationToken)
        {
            _logger.Information($"[{audioFile.UserId}] Start conversion audio file {audioFile.Id} to wav format");

            var sourceFileNamePath = Path.Combine(_diskStorage.GetDirectoryPath(), audioFile.SourceFileName ?? string.Empty);
            if (File.Exists(sourceFileNamePath))
            {
                _logger.Information($"[{audioFile.UserId}] Source wav file is already exists in destination in destination {sourceFileNamePath}");
                return;
            }

            var tempFilePath = string.Empty;
            try
            {
                var getBlobSettings = new GetBlobSettings(audioFile.OriginalSourceFileName, audioFile.UserId, audioFile.Id);
                var bloBytes = await _blobStorage.GetAsync(getBlobSettings, cancellationToken);
                tempFilePath = await _diskStorage.UploadAsync(bloBytes, cancellationToken);
                var (wavFilePath, fileName) = await ConvertToWavAsync(tempFilePath, audioFile.UserId);

                audioFile.SourceFileName = fileName;
                await _unitOfWork.SaveAsync(cancellationToken);

                _logger.Information($"[{audioFile.UserId}] Conversion audio file {audioFile.Id} to wav format finished. New audio file was stored in destination {wavFilePath}");
            }
            catch (RequestFailedException ex)
            {
                _logger.Error(ex, $"Blob storage is unavailable. User ID = {audioFile.Id}, Audio files = {audioFile.Id}, file name = {audioFile.OriginalSourceFileName}");
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

        public async Task<TranscribedAudioFile[]> SplitAudioFileAsync(AudioFile audioFile, CancellationToken cancellationToken)
        {
            var wavFilePath = Path.Combine(_diskStorage.GetDirectoryPath(), audioFile.SourceFileName ?? string.Empty);
            if (!File.Exists(wavFilePath))
                throw new FileNotFoundException($"Wav file {wavFilePath} does not exist");

            var remainingTime = await _currentUserSubscriptionRepository.GetRemainingTimeAsync(audioFile.UserId, cancellationToken);
            if (remainingTime < TimeSpan.FromSeconds(1))
                throw new InvalidOperationException($"User {audioFile.UserId} does not have enough free minutes in the subscription");

            var wavFileSource = await File.ReadAllBytesAsync(wavFilePath, cancellationToken);
            var transcribedAudioFiles = SplitWavFile(wavFileSource, remainingTime, audioFile.Id).ToArray();

            File.Delete(wavFilePath);

            return transcribedAudioFiles;
        }

        private async Task<(string filePath, string fileName)> ConvertToWavAsync(string inputFilePath, Guid userId)
        {
            var fileName = $"{Guid.NewGuid()}.voc";
            var filePath = Path.Combine(_diskStorage.GetDirectoryPath(), fileName);
            await Task.Run(() =>
            {
                using (var reader = new MediaFoundationReader(inputFilePath))
                {
                    WaveFileWriter.CreateWaveFile(filePath, reader);
                }
            });

            _logger.Information($"[{userId}] File {inputFilePath} was converted and stored in new destination {filePath}");

            return (filePath, fileName);
        }

        private IList<TranscribedAudioFile> SplitWavFile(byte[] inputFile, TimeSpan remainingTime, Guid audioFileId)
        {
            var transcribedAudioFiles = new List<TranscribedAudioFile>();
            var processedTime = TimeSpan.Zero;

            try
            {
                using (var stream = new MemoryStream(inputFile))
                using (var reader = new WaveFileReader(stream))
                {
                    var countItems = (int)Math.Floor(reader.TotalTime.TotalSeconds / FileLengthInSeconds);

                    for (var i = 0; i <= countItems; i++)
                    {
                        var processedSample = ProcessAudioSample(reader, remainingTime, processedTime, audioFileId);
                        if (processedSample.sampleDuration.Ticks <= 0)
                            return transcribedAudioFiles;

                        processedTime = processedTime.Add(processedSample.sampleDuration);
                        transcribedAudioFiles.Add(processedSample.transcribedAudioFile);
                    }

                    return transcribedAudioFiles;
                }
            }
            catch (Exception)
            {
                _logger.Error($"Remove wav audio files ({transcribedAudioFiles.Count}) from disk storage");

                if (transcribedAudioFiles.Any())
                {
                    foreach (var audioFile in transcribedAudioFiles)
                    {
                        if (File.Exists(audioFile.Path))
                            File.Delete(audioFile.Path);
                    }
                }

                throw;
            }
        }

        private (TimeSpan sampleDuration, TranscribedAudioFile transcribedAudioFile) ProcessAudioSample(WaveFileReader reader, TimeSpan remainingTime, TimeSpan processedTime, Guid audioFileId)
        {
            var remainingTimeSpan = remainingTime.Subtract(processedTime);
            if (remainingTimeSpan.Ticks <= 0)
                return (TimeSpan.MinValue, null);

            var sampleDuration = remainingTimeSpan.TotalSeconds < FileLengthInSeconds
                ? remainingTimeSpan
                : TimeSpan.FromSeconds(FileLengthInSeconds);

            var audioTotalTime = reader.TotalTime;
            var end = processedTime.Add(sampleDuration);
            var endTime = end > audioTotalTime ? audioTotalTime : end;

            var transcribedAudioFile = CreateTranscribedAudioFile(reader, processedTime, endTime, audioFileId);

            return (sampleDuration, transcribedAudioFile);
        }

        private TranscribedAudioFile CreateTranscribedAudioFile(WaveFileReader reader, TimeSpan start, TimeSpan end, Guid audioFileId)
        {
            var outputFileName = Path.Combine(_diskStorage.GetDirectoryPath(), $"{Guid.NewGuid()}.voc");
            using (var writer = new WaveFileWriter(outputFileName, reader.WaveFormat))
            {
                var fileSegmentLength = reader.WaveFormat.AverageBytesPerSecond / 1000;

                var startPosition = (int)start.TotalMilliseconds * fileSegmentLength;
                startPosition = startPosition - startPosition % reader.WaveFormat.BlockAlign;

                var endPosition = (int)end.TotalMilliseconds * fileSegmentLength;
                endPosition = endPosition - endPosition % reader.WaveFormat.BlockAlign;

                reader.Position = startPosition;
                var buffer = new byte[1024];

                while (reader.Position < endPosition)
                {
                    var currentSegmentLength = (int)(endPosition - reader.Position);
                    if (currentSegmentLength > 0)
                    {
                        var bytesToRead = Math.Min(currentSegmentLength, buffer.Length);
                        var readBytes = reader.Read(buffer, 0, bytesToRead);
                        if (readBytes > 0)
                        {
                            writer.Write(buffer, 0, readBytes);
                        }
                    }
                }

                var wavPartialFile = new TranscribedAudioFile
                {
                    Id = Guid.NewGuid(),
                    AudioFileId = audioFileId,
                    Path = outputFileName,
                    AudioChannels = reader.WaveFormat.Channels,
                    StartTime = start,
                    EndTime = end,
                    TotalTime = writer.TotalTime
                };

                return wavPartialFile;
            }
        }
    }
}
