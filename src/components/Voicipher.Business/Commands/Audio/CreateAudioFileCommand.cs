using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Serilog;
using Voicipher.Business.Extensions;
using Voicipher.Business.Infrastructure;
using Voicipher.Business.Utils;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Commands.Audio;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Models;
using Voicipher.Domain.OutputModels.Audio;
using Voicipher.Domain.Payloads.Audio;
using Voicipher.Domain.Validation;

namespace Voicipher.Business.Commands.Audio
{
    public class CreateAudioFileCommand : Command<CreateAudioFilePayload, CommandResult<FileItemOutputModel>>, ICreateAudioFileCommand
    {
        private readonly IMessageCenterService _messageCenterService;
        private readonly IAudioFileRepository _audioFileRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CreateAudioFileCommand(
            IMessageCenterService messageCenterService,
            IAudioFileRepository audioFileRepository,
            IMapper mapper,
            ILogger logger)
        {
            _messageCenterService = messageCenterService;
            _audioFileRepository = audioFileRepository;
            _mapper = mapper;
            _logger = logger.ForContext<CreateAudioFileCommand>();
        }

        protected override async Task<CommandResult<FileItemOutputModel>> Execute(CreateAudioFilePayload parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var validationResult = parameter.Validate();
            if (!validationResult.IsValid)
            {
                if (validationResult.Errors.ContainsError(nameof(CreateAudioFilePayload.Language), ValidationErrorCodes.NotSupportedLanguage))
                {
                    _logger.Error($"Language {parameter.Language} is not supported.");

                    throw new OperationErrorException(ErrorCode.EC200);
                }

                _logger.Error("Invalid input data.");

                throw new OperationErrorException(ErrorCode.EC600);
            }

            var userId = principal.GetNameIdentifier();
            var audioFile = new AudioFile
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ApplicationId = parameter.ApplicationId,
                Name = parameter.Name,
                FileName = parameter.FileName,
                Language = parameter.Language,
                Storage = StorageSetting.Azure,
                DateCreated = parameter.DateCreated,
                DateUpdatedUtc = DateTime.UtcNow
            };

            await _audioFileRepository.AddAsync(audioFile);
            await _messageCenterService.SendAsync(HubMethodsHelper.GetFilesListChangedMethod(userId));
            await _audioFileRepository.SaveAsync(cancellationToken);

            _logger.Information($"Audio file was created. Audio file ID = {audioFile.Id}, name = {audioFile.Name}, file name = {audioFile.FileName}, user ID = {userId}");

            return new CommandResult<FileItemOutputModel>(_mapper.Map<FileItemOutputModel>(audioFile));
        }
    }
}
