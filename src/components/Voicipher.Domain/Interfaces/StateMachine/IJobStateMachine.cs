using System;
using System.Threading;
using System.Threading.Tasks;
using Voicipher.Domain.Models;

namespace Voicipher.Domain.Interfaces.StateMachine
{
    public interface IJobStateMachine
    {
        public IMachineState MachineState { get; }

        Task DoInitAsync(BackgroundJob backgroundJob, CancellationToken cancellationToken);

        Task DoValidationAsync(CancellationToken cancellationToken);

        Task DoConvertingAsync(CancellationToken cancellationToken);

        Task DoSplitAsync(CancellationToken cancellationToken);

        Task DoProcessingAsync(CancellationToken cancellationToken);

        Task DoCompleteAsync(CancellationToken cancellationToken);

        Task DoErrorAsync(Exception exception, CancellationToken cancellationToken);

        Task SaveAsync(CancellationToken cancellationToken);

        void DoClean();
    }
}
