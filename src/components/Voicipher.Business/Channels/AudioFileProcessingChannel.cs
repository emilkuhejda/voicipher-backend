using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Voicipher.Domain.Interfaces.Channels;
using Voicipher.Domain.Models;

namespace Voicipher.Business.Channels
{
    public class AudioFileProcessingChannel : IAudioFileProcessingChannel
    {
        private readonly Channel<RecognitionFile> _channel;

        public AudioFileProcessingChannel()
        {
            _channel = Channel.CreateUnbounded<RecognitionFile>();
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
                    return true;
                }
            }

            return false;
        }
    }
}
