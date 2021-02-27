using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Commands.ControlPanel;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.OutputModels.ControlPanel;
using Voicipher.Domain.Payloads.ControlPanel;

namespace Voicipher.Business.Commands.ControlPanel
{
    public class CleanUpTranscribeItemsCommand : CleanUpBaseCommand<CleanUpTranscribeItemsPayload, CommandResult<CleanUpTranscribeItemsOutputModel>>, ICleanUpTranscribeItemsCommand
    {
        public CleanUpTranscribeItemsCommand(IAudioFileRepository audioFileRepository, ILogger logger)
            : base(audioFileRepository, logger.ForContext<CleanUpTranscribeItemsCommand>())
        {
        }

        protected override async Task<CommandResult<CleanUpTranscribeItemsOutputModel>> Execute(CleanUpTranscribeItemsPayload parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var deleteBefore = DateTime.UtcNow.AddDays(0);
            var audioFiles = await AudioFileRepository.GetAllForCleanUpAsync(deleteBefore, cancellationToken);
            if (!audioFiles.Any())
            {
                Logger.Information("No audio files for cleanup");
                return new CommandResult<CleanUpTranscribeItemsOutputModel>(new CleanUpTranscribeItemsOutputModel());
            }

            return new CommandResult<CleanUpTranscribeItemsOutputModel>(new CleanUpTranscribeItemsOutputModel());
        }
    }
}
