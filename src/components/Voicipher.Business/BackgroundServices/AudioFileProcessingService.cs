using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Interfaces.Channels;
using Voicipher.Domain.Interfaces.Commands.Job;
using Voicipher.Domain.Payloads.Job;

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
                _logger.Information($"Recognition file {JsonConvert.SerializeObject(recognitionFile)} was started to process");

                using (var scope = _serviceProvider.CreateScope())
                {
                    var createBackgroundJobCommand = scope.ServiceProvider.GetRequiredService<ICreateBackgroundJobCommand>();
                    var runBackgroundJobCommand = scope.ServiceProvider.GetRequiredService<IRunBackgroundJobCommand>();

                    var parameters = new Dictionary<BackgroundJobParameter, object> { { BackgroundJobParameter.DateUtc, recognitionFile.DateProcessedUtc } };
                    var createBackgroundJobPayload = new CreateBackgroundJobPayload(recognitionFile.UserId, recognitionFile.AudioFileId, parameters);
                    var commandResult = await createBackgroundJobCommand.ExecuteAsync(createBackgroundJobPayload, null, stoppingToken);

                    if (commandResult.IsSuccess)
                    {
                        Task.Run(async () =>
                        {
                            try
                            {
                                await runBackgroundJobCommand.ExecuteAsync(commandResult.Value, null, stoppingToken);
                            }
                            catch (Exception ex)
                            {
                                _logger.Fatal(ex, "Background job failed");
                            }
                        });
                    }
                }
            }
        }
    }
}
