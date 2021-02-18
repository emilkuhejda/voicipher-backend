using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Serilog;
using Voicipher.Business.Extensions;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.InputModels;
using Voicipher.Domain.Interfaces.Commands;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;
using Voicipher.Domain.OutputModels;

namespace Voicipher.Business.Commands
{
    public class CreateSpeechResultCommand : Command<CreateSpeechResultInputModel, CommandResult<OkOutputModel>>, ICreateSpeechResultCommand
    {
        private readonly IRecognizedAudioSampleRepository _recognizedAudioSampleRepository;
        private readonly ISpeechResultRepository _speechResultRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CreateSpeechResultCommand(
            IRecognizedAudioSampleRepository recognizedAudioSampleRepository,
            ISpeechResultRepository speechResultRepository,
            IMapper mapper,
            ILogger logger)
        {
            _recognizedAudioSampleRepository = recognizedAudioSampleRepository;
            _speechResultRepository = speechResultRepository;
            _mapper = mapper;
            _logger = logger.ForContext<CreateSpeechResultCommand>();
        }

        protected override async Task<CommandResult<OkOutputModel>> Execute(CreateSpeechResultInputModel parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            if (!parameter.Validate().IsValid)
            {
                _logger.Error("Invalid input data");

                throw new OperationErrorException(ErrorCode.EC600);
            }

            var userId = principal.GetNameIdentifier();
            var audioSample = await _recognizedAudioSampleRepository.GetAsync(parameter.RecognizedAudioSampleId, cancellationToken);
            if (audioSample == null)
            {
                _logger.Error($"Recognized audio sample {parameter.RecognizedAudioSampleId} not found for user {userId}");

                throw new OperationErrorException(ErrorCode.EC105);
            }

            var speechResult = _mapper.Map<SpeechResult>(parameter);

            await _speechResultRepository.AddAsync(speechResult);
            await _speechResultRepository.SaveAsync(cancellationToken);

            _logger.Information($"User with ID = {userId} inserted speech result");

            return new CommandResult<OkOutputModel>(new OkOutputModel());
        }
    }
}
