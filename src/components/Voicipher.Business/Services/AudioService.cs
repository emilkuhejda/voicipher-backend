using System;
using NAudio.Wave;
using Serilog;
using Voicipher.Domain.Interfaces.Services;

namespace Voicipher.Business.Services
{
    public class AudioService : IAudioService
    {
        private readonly ILogger _logger;

        public AudioService(ILogger logger)
        {
            _logger = logger.ForContext<AudioService>();
        }

        public TimeSpan? GetTotalTime(string filePath)
        {
            try
            {
                using (var reader = new MediaFoundationReader(filePath))
                {
                    return reader.TotalTime;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Cannot read audio file in destination {filePath}");

                return null;
            }
        }
    }
}
