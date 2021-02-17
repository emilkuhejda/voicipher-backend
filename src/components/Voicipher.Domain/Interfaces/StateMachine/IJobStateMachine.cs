using System.Threading;
using System.Threading.Tasks;
using Voicipher.Domain.Models;

namespace Voicipher.Domain.Interfaces.StateMachine
{
    public interface IJobStateMachine
    {
        void DoInit(BackgroundJob backgroundJob);

        Task DoValidationAsync(CancellationToken cancellationToken);

        Task DoConvertingAsync(CancellationToken cancellationToken);

        Task DoProcessingAsync(CancellationToken cancellationToken);

        void DoCompleteAsync(CancellationToken cancellationToken);

        Task SaveAsync(CancellationToken cancellationToken);
    }
}
