using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Voicipher.Domain.Interfaces.Channels;
using Voicipher.Domain.Models;

namespace Voicipher.Business.Channels
{
    public class MailProcessingChannel : IMailProcessingChannel
    {
        private readonly Channel<MailData> _channel;

        public MailProcessingChannel()
        {
            var options = new BoundedChannelOptions(100)
            {
                SingleWriter = true,
                SingleReader = true
            };

            _channel = Channel.CreateBounded<MailData>(options);
        }

        public IAsyncEnumerable<MailData> ReadAllAsync(CancellationToken cancellationToken) => _channel.Reader.ReadAllAsync(cancellationToken);

        public async Task<bool> AddFileAsync(MailData mailData, CancellationToken cancellationToken)
        {
            while (await _channel.Writer.WaitToWriteAsync(cancellationToken) && !cancellationToken.IsCancellationRequested)
            {
                if (_channel.Writer.TryWrite(mailData))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
