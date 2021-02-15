using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.InputModels;
using Voicipher.Domain.Interfaces.Commands;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.OutputModels;

namespace Voicipher.Business.Commands
{
    public class UpdateUserTranscriptCommand : Command<UpdateUserTranscriptInputModel, CommandResult<OkOutputModel>>, IUpdateUserTranscriptCommand
    {
        private readonly ITranscribeItemRepository _transcribeItemRepository;
        private readonly ILogger _logger;

        public UpdateUserTranscriptCommand(
            ITranscribeItemRepository transcribeItemRepository,
            ILogger logger)
        {
            _transcribeItemRepository = transcribeItemRepository;
            _logger = logger.ForContext<UpdateUserTranscriptCommand>();
        }

        protected override Task<CommandResult<OkOutputModel>> Execute(UpdateUserTranscriptInputModel parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
        }
    }
}
