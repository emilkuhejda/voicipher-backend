using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Infrastructure;
using Voicipher.Domain.Payloads.Job;

namespace Voicipher.Domain.Interfaces.Commands.Job
{
    public interface ICreateBackgroundJobCommand : ICommand<CreateBackgroundJobPayload, CommandResult<BackgroundJobPayload>>
    {
    }
}
