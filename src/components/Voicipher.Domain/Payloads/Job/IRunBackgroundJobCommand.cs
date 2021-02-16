using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Infrastructure;

namespace Voicipher.Domain.Payloads.Job
{
    public interface IRunBackgroundJobCommand : ICommand<BackgroundJobPayload, CommandResult>
    {
    }
}
