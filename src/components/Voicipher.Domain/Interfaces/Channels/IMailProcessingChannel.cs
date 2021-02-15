using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Voicipher.Domain.Models;

namespace Voicipher.Domain.Interfaces.Channels
{
    public interface IMailProcessingChannel
    {
        IAsyncEnumerable<MailData> ReadAllAsync(CancellationToken cancellationToken = default);

        Task<bool> AddFileAsync(MailData mailData, CancellationToken cancellationToken = default);
    }
}
