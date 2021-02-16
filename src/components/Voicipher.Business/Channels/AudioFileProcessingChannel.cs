using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serilog;
using Voicipher.Domain.Interfaces.Channels;
using Voicipher.Domain.Models;

namespace Voicipher.Business.Channels
{
    public class AudioFileProcessingChannel : IAudioFileProcessingChannel
    {
        private readonly Channel<RecognitionFile> _channel;
        private readonly ILogger _logger;

        public AudioFileProcessingChannel(ILogger logger)
        {
            _channel = Channel.CreateUnbounded<RecognitionFile>();
            _logger = logger.ForContext<AudioFileProcessingChannel>();
        }

        public IAsyncEnumerable<RecognitionFile> ReadAllAsync(CancellationToken cancellationToken = default)
        {
            return _channel.Reader.ReadAllAsync(cancellationToken);
        }

        public async Task<bool> AddFileAsync(RecognitionFile recognitionFile, CancellationToken cancellationToken = default)
        {
            while (await _channel.Writer.WaitToWriteAsync(cancellationToken) && !cancellationToken.IsCancellationRequested)
            {
                if (_channel.Writer.TryWrite(recognitionFile))
                {
                    _logger.Information($"Recognition file {JsonConvert.SerializeObject(recognitionFile)} was written to the channel");

                    return true;
                }
            }

            return false;
        }
    }
}
