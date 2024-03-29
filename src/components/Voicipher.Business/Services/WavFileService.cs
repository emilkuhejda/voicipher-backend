﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using Azure;
using NAudio.Wave;
using Serilog;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Models;
using Voicipher.Domain.Utils;

namespace Voicipher.Business.Services
{
    public class WavFileService : IWavFileService
    {
        private const float ExtraSeconds = 0.25f;

        private readonly IFileAccessService _fileAccessService;
        private readonly IBlobStorage _blobStorage;
        private readonly IDiskStorage _diskStorage;
        private readonly ICurrentUserSubscriptionRepository _currentUserSubscriptionRepository;
        private readonly ILogger _logger;

        public WavFileService(
            IFileAccessService fileAccessService,
            IBlobStorage blobStorage,
            IIndex<StorageLocation, IDiskStorage> index,
            ICurrentUserSubscriptionRepository currentUserSubscriptionRepository,
            ILogger logger)
        {
            _fileAccessService = fileAccessService;
            _blobStorage = blobStorage;
            _diskStorage = index[StorageLocation.Audio];
            _currentUserSubscriptionRepository = currentUserSubscriptionRepository;
            _logger = logger.ForContext<WavFileService>();
        }

        public async Task<string> RunConversionToWavAsync(AudioFile audioFile, CancellationToken cancellationToken)
        {
            _logger.Information($"[{audioFile.UserId}] Start conversion audio file {audioFile.Id} to wav format");

            var sourceFileNamePath = Path.Combine(GetDirectoryPath(audioFile.Id), audioFile.SourceFileName ?? string.Empty);
            if (_fileAccessService.Exists(sourceFileNamePath))
            {
                _logger.Error($"[{audioFile.UserId}] Source wav file is already exists in destination in destination {sourceFileNamePath}");
                return string.Empty;
            }

            var tempFilePath = string.Empty;
            try
            {
                _logger.Verbose($"[{audioFile.UserId}] Start downloading audio file {audioFile.OriginalSourceFileName} from blob storage");

                var getBlobSettings = new GetBlobSettings(audioFile.OriginalSourceFileName, audioFile.UserId, audioFile.Id);
                var bloBytes = await _blobStorage.GetAsync(getBlobSettings, cancellationToken);
                tempFilePath = await _diskStorage.UploadAsync(bloBytes, cancellationToken);

                _logger.Verbose($"[{audioFile.UserId}] Audio file {audioFile.OriginalSourceFileName} was downloaded from blob storage");

                var wavFilePath = await ConvertToWavAsync(tempFilePath, audioFile);

                _logger.Information($"[{audioFile.UserId}] Conversion audio file {audioFile.Id} to wav format finished. New audio file was stored in destination {wavFilePath}");

                return Path.GetFileName(wavFilePath);
            }
            catch (RequestFailedException ex)
            {
                _logger.Error(ex, $"[{audioFile.UserId}] Blob storage is unavailable. Audio file = {audioFile.Id}, file name = {audioFile.OriginalSourceFileName}");
                throw;
            }
            finally
            {
                _fileAccessService.Delete(tempFilePath);
            }
        }

        public async Task<TranscribedAudioFile[]> SplitAudioFileAsync(AudioFile audioFile, CancellationToken cancellationToken)
        {
            var wavFilePath = Path.Combine(GetDirectoryPath(audioFile.Id), audioFile.SourceFileName ?? string.Empty);
            if (!_fileAccessService.Exists(wavFilePath))
                throw new FileNotFoundException($"Wav file {wavFilePath} does not exist");

            _logger.Verbose($"[{audioFile.UserId}] Clean temporary partial files");
            var directory = GetDirectoryPath(audioFile.Id);
            foreach (var path in _fileAccessService.GetFiles(directory).Where(x => !x.Equals(wavFilePath, StringComparison.OrdinalIgnoreCase)))
            {
                _fileAccessService.Delete(path);
            }

            var remainingTime = await _currentUserSubscriptionRepository.GetRemainingTimeAsync(audioFile.UserId, cancellationToken);
            if (remainingTime < TimeSpan.FromSeconds(1))
                throw new InvalidOperationException($"User {audioFile.UserId} does not have enough free minutes in the subscription");

            var wavFileSource = await _fileAccessService.ReadAllBytesAsync(wavFilePath, cancellationToken);
            var transcribedAudioFiles = await SplitWavFileAsync(wavFileSource, remainingTime, audioFile.Id, audioFile.UserId);

            return transcribedAudioFiles.ToArray();
        }

        private async Task<string> ConvertToWavAsync(string inputFilePath, AudioFile audioFile)
        {
            _logger.Verbose($"[{audioFile.UserId}] Start conversion audio file to wav format");

            using (var reader = new MediaFoundationReader(inputFilePath))
            {
                var endTime = Math.Min(audioFile.TranscriptionEndTime.Ticks, audioFile.TotalTime.Ticks);
                var destinationFileName = GetFilePath(audioFile.Id);
                var trimmedAudioFile = await TrimAudioFileAsync(reader, audioFile.TranscriptionStartTime, TimeSpan.FromTicks(endTime), destinationFileName);

                _logger.Verbose($"[{audioFile.UserId}] File {inputFilePath} was converted and stored in new destination {trimmedAudioFile.filePath}");

                return trimmedAudioFile.filePath;
            }
        }

        private async Task<IList<TranscribedAudioFile>> SplitWavFileAsync(byte[] inputFile, TimeSpan remainingTime, Guid audioFileId, Guid userId)
        {
            var transcribedAudioFiles = new List<TranscribedAudioFile>();
            var processedTime = TimeSpan.Zero;

            try
            {
                using (var stream = new MemoryStream(inputFile))
                using (var reader = new WaveFileReader(stream))
                {
                    var fileLengthInSeconds = CalculatePartialFileLengthInSeconds(inputFile.Length, reader.TotalTime.TotalSeconds);
                    var countItems = (int)Math.Floor(reader.TotalTime.TotalSeconds / fileLengthInSeconds);
                    var audioTotalTimeTicks = Math.Min(reader.TotalTime.Ticks, remainingTime.Ticks);
                    var audioTotalTime = TimeSpan.FromTicks(audioTotalTimeTicks);

                    for (var i = 0; i <= countItems; i++)
                    {
                        var remainingTimeSpan = remainingTime.Subtract(processedTime);
                        if (remainingTimeSpan.Ticks <= 0)
                            return transcribedAudioFiles;

                        var sampleDuration = remainingTimeSpan.TotalSeconds < fileLengthInSeconds
                            ? remainingTimeSpan
                            : TimeSpan.FromSeconds(fileLengthInSeconds);

                        var requestedEndTime = processedTime.Add(sampleDuration).Add(TimeSpan.FromSeconds(ExtraSeconds));
                        var endTime = requestedEndTime > audioTotalTime ? audioTotalTime : requestedEndTime;
                        var destinationFileName = GetFilePath(audioFileId);
                        var trimmedAudioFile = await TrimAudioFileAsync(reader, processedTime, endTime, destinationFileName);

                        var transcribedAudioFile = new TranscribedAudioFile
                        {
                            Id = Guid.NewGuid(),
                            AudioFileId = audioFileId,
                            Path = trimmedAudioFile.filePath,
                            AudioChannels = reader.WaveFormat.Channels,
                            StartTime = processedTime,
                            EndTime = endTime,
                            TotalTime = trimmedAudioFile.totalTime
                        };

                        processedTime = processedTime.Add(sampleDuration);
                        transcribedAudioFiles.Add(transcribedAudioFile);
                    }

                    return transcribedAudioFiles;
                }
            }
            catch (Exception)
            {
                _logger.Error($"[{userId}] Remove wav audio files ({transcribedAudioFiles.Count}) from disk storage");

                if (transcribedAudioFiles.Any())
                {
                    foreach (var audioFile in transcribedAudioFiles)
                    {
                        _fileAccessService.Delete(audioFile.Path);
                    }
                }

                throw;
            }
        }

        private Task<(string filePath, TimeSpan totalTime)> TrimAudioFileAsync(WaveStream reader, TimeSpan startTime, TimeSpan endTime, string destinationFileName)
        {
            return Task.Run(() =>
            {
                using (var writer = new WaveFileWriter(destinationFileName, reader.WaveFormat))
                {
                    var fileSegmentLength = reader.WaveFormat.AverageBytesPerSecond / 1000;

                    var startPosition = (int)startTime.TotalMilliseconds * fileSegmentLength;
                    startPosition = startPosition - startPosition % reader.WaveFormat.BlockAlign;

                    var endPosition = (int)endTime.TotalMilliseconds * fileSegmentLength;
                    endPosition = endPosition - endPosition % reader.WaveFormat.BlockAlign;

                    reader.Position = startPosition;
                    var buffer = new byte[1024];

                    while (reader.Position < endPosition)
                    {
                        var currentSegmentLength = (int)(endPosition - reader.Position);
                        if (currentSegmentLength <= 0)
                            break;

                        var bytesToRead = Math.Min(currentSegmentLength, buffer.Length);
                        var readBytes = reader.Read(buffer, 0, bytesToRead);
                        if (readBytes <= 0)
                            break;

                        writer.Write(buffer, 0, readBytes);
                    }

                    return (destinationFileName, writer.TotalTime);
                }
            });
        }

        private string GetFilePath(Guid audioFileId)
        {
            return Path.Combine(GetDirectoryPath(audioFileId), $"{Guid.NewGuid()}{MimeTypes.VocExtension}");
        }

        private string GetDirectoryPath(Guid audioFileId)
        {
            return _diskStorage.GetDirectoryPath(audioFileId.ToString());
        }

        private double CalculatePartialFileLengthInSeconds(int bytes, double totalSeconds)
        {
            const int maxBytesThreshold = 10000000;
            const int fileLengthInSeconds = 58;

            var bytesPerSeconds = bytes / totalSeconds;
            var secondsPerFile = (maxBytesThreshold / bytesPerSeconds) - ExtraSeconds;
            return Math.Min(Math.Floor(secondsPerFile), fileLengthInSeconds);
        }
    }
}
