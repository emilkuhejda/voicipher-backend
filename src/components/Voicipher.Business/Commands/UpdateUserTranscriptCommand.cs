using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Exceptions;
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

        protected override async Task<CommandResult<OkOutputModel>> Execute(UpdateUserTranscriptInputModel parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            if (!parameter.Validate().IsValid)
            {
                _logger.Error("Invalid input data");

                throw new OperationErrorException(ErrorCode.EC600);
            }

            var transcribeItem = await _transcribeItemRepository.GetAsync(parameter.TranscribeItemId, cancellationToken);
            if (transcribeItem == null)
            {
                _logger.Error($"Transcribe item {parameter.TranscribeItemId} not found");

                throw new OperationErrorException(ErrorCode.EC101);
            }

            transcribeItem.UserTranscript = parameter.Transcript;
            transcribeItem.ApplicationId = parameter.ApplicationId;
            transcribeItem.DateUpdatedUtc = DateTime.UtcNow;

            await _transcribeItemRepository.SaveAsync(cancellationToken);

            _logger.Information($"Transcribe item {parameter.TranscribeItemId} was updated");

            return new CommandResult<OkOutputModel>(new OkOutputModel());
        }
    }
}
