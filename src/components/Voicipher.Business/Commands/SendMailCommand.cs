using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Voicipher.Business.Extensions;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.InputModels;
using Voicipher.Domain.Interfaces.Channels;
using Voicipher.Domain.Interfaces.Commands;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;
using Voicipher.Domain.OutputModels;
using Voicipher.Domain.Validation;

namespace Voicipher.Business.Commands
{
    public class SendMailCommand : Command<SendMailInputModel, CommandResult<OkOutputModel>>, ISendMailCommand
    {
        private const string Transcription = "Transcription";

        private readonly IMailProcessingChannel _mailProcessingChannel;
        private readonly IAudioFileRepository _audioFileRepository;
        private readonly ILogger _logger;

        public SendMailCommand(
            IMailProcessingChannel mailProcessingChannel,
            IAudioFileRepository audioFileRepository,
            ILogger logger)
        {
            _mailProcessingChannel = mailProcessingChannel;
            _audioFileRepository = audioFileRepository;
            _logger = logger.ForContext<SendMailCommand>();
        }

        protected override async Task<CommandResult<OkOutputModel>> Execute(SendMailInputModel parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var validationResult = parameter.Validate();
            if (!validationResult.IsValid)
            {
                if (validationResult.Errors.ContainsError(nameof(SendMailInputModel.Recipient), ValidationErrorCodes.InvalidEmail))
                {
                    _logger.Error($"Recipient email address format '{parameter.Recipient}' is incorrect.");

                    throw new OperationErrorException(ErrorCode.EC202);
                }

                _logger.Error("Invalid input data.");

                throw new OperationErrorException(ErrorCode.EC600);
            }

            var userId = principal.GetNameIdentifier();
            var audioFile = await _audioFileRepository.GetWithTranscribeItemsAsync(userId, parameter.FileItemId, cancellationToken);
            if (audioFile == null)
            {
                _logger.Error($"Audio file '{parameter.FileItemId}' was not found.");

                throw new OperationErrorException(ErrorCode.EC101);
            }

            var body = new StringBuilder();
            foreach (var transcribeItem in audioFile.TranscribeItems.OrderBy(x => x.StartTime))
            {
                var transcript = string.IsNullOrWhiteSpace(transcribeItem.UserTranscript)
                    ? string.Join(string.Empty, transcribeItem.GetAlternatives().Select(x => x.Transcript))
                    : transcribeItem.UserTranscript;

                body.AppendLine(transcript);
                body.AppendLine();
            }

            var subject = $"{Transcription}: {audioFile.Name}";

            await _mailProcessingChannel.AddFileAsync(new MailData(parameter.Recipient, subject, body.ToString()));

            _logger.Information("Email was sent to queue");

            return new CommandResult<OkOutputModel>(new OkOutputModel());
        }
    }
}
