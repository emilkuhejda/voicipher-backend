using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Interfaces.Channels;
using Voicipher.Domain.Interfaces.Commands.Transcription;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;
using Voicipher.Domain.Payloads.Transcription;
using Voicipher.Domain.Settings;

namespace Voicipher.Business.BackgroundServices
{
    public class RestoreRecognitionStateService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IAudioFileProcessingChannel _audioFileProcessingChannel;
        private readonly ILogger _logger;

        public RestoreRecognitionStateService(
            IServiceProvider serviceProvider,
            IAudioFileProcessingChannel audioFileProcessingChannel,
            ILogger logger)
        {
            _serviceProvider = serviceProvider;
            _audioFileProcessingChannel = audioFileProcessingChannel;
            _logger = logger.ForContext<IAudioFileProcessingChannel>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();

            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var options = scope.ServiceProvider.GetRequiredService<IOptions<AppSettings>>();
                    var audioFileRepository = scope.ServiceProvider.GetRequiredService<IAudioFileRepository>();
                    var audioFiles = await audioFileRepository.GetInProgressAsync(stoppingToken);

                    _logger.Information($"There were found {audioFiles.Length} audio files in recognition state {RecognitionState.InProgress}");

                    foreach (var audioFile in audioFiles)
                    {
                        var updateRecognitionStateCommand = scope.ServiceProvider.GetRequiredService<IUpdateRecognitionStateCommand>();
                        var updateRecognitionStatePayload = new UpdateRecognitionStatePayload(audioFile.Id, audioFile.UserId, options.Value.ApplicationId, RecognitionState.None);
                        await updateRecognitionStateCommand.ExecuteAsync(updateRecognitionStatePayload, null, stoppingToken);

                        _logger.Information($"Try to restart transcription operation for audio file {audioFile.Id}");

                        var recognitionFile = new RecognitionFile(audioFile.UserId, audioFile.Id, audioFile.FileName);
                        await _audioFileProcessingChannel.AddFileAsync(recognitionFile, stoppingToken);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex, "Background job initialization failed");
            }
        }
    }
}
