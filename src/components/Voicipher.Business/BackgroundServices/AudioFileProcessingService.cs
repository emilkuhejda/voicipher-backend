using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Channels;
using Voicipher.Domain.Interfaces.Commands.Job;
using Voicipher.Domain.Interfaces.Commands.Transcription;
using Voicipher.Domain.Payloads.Job;
using Voicipher.Domain.Payloads.Transcription;
using Voicipher.Domain.Settings;
using Voicipher.Domain.Validation;

namespace Voicipher.Business.BackgroundServices
{
    public class AudioFileProcessingService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IAudioFileProcessingChannel _audioFileProcessingChannel;
        private readonly ILogger _logger;

        public AudioFileProcessingService(
            IServiceProvider serviceProvider,
            IAudioFileProcessingChannel audioFileProcessingChannel,
            ILogger logger)
        {
            _serviceProvider = serviceProvider;
            _audioFileProcessingChannel = audioFileProcessingChannel;
            _logger = logger.ForContext<AudioFileProcessingService>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var recognitionFile in _audioFileProcessingChannel.ReadAllAsync(stoppingToken))
            {
                _logger.Information($"Recognition file {JsonConvert.SerializeObject(recognitionFile)} was started processing");

                CommandResult<BackgroundJobPayload> createJobCommandResult;
                var isSuccess = true;

                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var createBackgroundJobCommand = scope.ServiceProvider.GetRequiredService<ICreateBackgroundJobCommand>();
                        var updateRecognitionStateCommand = scope.ServiceProvider.GetRequiredService<IUpdateRecognitionStateCommand>();
                        var appSettings = scope.ServiceProvider.GetRequiredService<IOptions<AppSettings>>().Value;

                        var parameters = new Dictionary<BackgroundJobParameter, object> { { BackgroundJobParameter.DateUtc, recognitionFile.DateProcessedUtc } };
                        var createBackgroundJobPayload = new CreateBackgroundJobPayload(recognitionFile.UserId, recognitionFile.AudioFileId, parameters);
                        createJobCommandResult = await createBackgroundJobCommand.ExecuteAsync(createBackgroundJobPayload, null, stoppingToken);
                        isSuccess &= createJobCommandResult.IsSuccess;

                        var payload = new UpdateRecognitionStatePayload(recognitionFile.AudioFileId, recognitionFile.UserId, appSettings.ApplicationId, RecognitionState.InProgress);
                        var updateStateCommandResult = await updateRecognitionStateCommand.ExecuteAsync(payload, null, stoppingToken);
                        isSuccess &= updateStateCommandResult.IsSuccess;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Fatal(ex, "Background job initialization failed");

                    createJobCommandResult = new CommandResult<BackgroundJobPayload>(new OperationError(ValidationErrorCodes.OperationFailed));
                    isSuccess = false;
                }


                if (isSuccess)
                {
                    Task.Run(async () =>
                    {
                        try
                        {
                            using (var scope = _serviceProvider.CreateScope())
                            {
                                var runBackgroundJobCommand = scope.ServiceProvider.GetRequiredService<IRunBackgroundJobCommand>();
                                await runBackgroundJobCommand.ExecuteAsync(createJobCommandResult.Value, null, stoppingToken);
                            }
                        }
                        catch (Exception ex)
                        {
                            _audioFileProcessingChannel.FinishProcessing(recognitionFile);
                            _logger.Fatal(ex, "Background job failed");
                        }
                    });
                }
            }
        }
    }
}
