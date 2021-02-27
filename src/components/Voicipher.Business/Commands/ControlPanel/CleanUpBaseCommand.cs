using Serilog;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Interfaces.Repositories;

namespace Voicipher.Business.Commands.ControlPanel
{
    public abstract class CleanUpBaseCommand<T, TResult> : Command<T, TResult>
    {
        protected CleanUpBaseCommand(IAudioFileRepository audioFileRepository, ILogger logger)
        {
            AudioFileRepository = audioFileRepository;
            Logger = logger;
        }

        protected IAudioFileRepository AudioFileRepository { get; }

        protected ILogger Logger { get; }
    }
}
