using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;
using Voicipher.Domain.Interfaces.Channels;
using Voicipher.Domain.Models;

namespace Voicipher.Business.BackgroundServices
{
    public class AudioFileProcessingService : BackgroundService
    {
        private readonly IAudioFileProcessingChannel _audioFileProcessingChannel;
        private readonly ILogger _logger;

        public AudioFileProcessingService(
            IAudioFileProcessingChannel audioFileProcessingChannel,
            ILogger logger)
        {
            _audioFileProcessingChannel = audioFileProcessingChannel;
            _logger = logger.ForContext<AudioFileProcessingService>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var recognitionFile in _audioFileProcessingChannel.ReadAllAsync(stoppingToken))
            {
                _logger.Information($"Recognition file {JsonConvert.SerializeObject(recognitionFile)} was started to process");

                await Task.Delay(TimeSpan.FromSeconds(20));
            }
        }
    }
}
