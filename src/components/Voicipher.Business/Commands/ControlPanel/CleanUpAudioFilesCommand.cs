using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using Serilog;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Commands.ControlPanel;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.OutputModels.ControlPanel;
using Voicipher.Domain.Payloads.ControlPanel;

namespace Voicipher.Business.Commands.ControlPanel
{
    public class CleanUpAudioFilesCommand : Command<CleanUpAudioFilesPayload, CommandResult<CleanUpAudioFilesOutputModel>>, ICleanUpAudioFilesCommand
    {
        private readonly IAudioFileRepository _audioFileRepository;
        private readonly IDiskStorage _diskStorage;
        private readonly ILogger _logger;

        public CleanUpAudioFilesCommand(
            IAudioFileRepository audioFileRepository,
            IIndex<StorageLocation, IDiskStorage> index,
            ILogger logger)
        {
            _audioFileRepository = audioFileRepository;
            _diskStorage = index[StorageLocation.Backup];
            _logger = logger;
        }

        protected override async Task<CommandResult<CleanUpAudioFilesOutputModel>> Execute(CleanUpAudioFilesPayload parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
