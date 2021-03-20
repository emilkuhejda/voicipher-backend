﻿using System;
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
using Voicipher.Domain.InputModels.Audio;
using Voicipher.Domain.Interfaces.Commands.Audio;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.OutputModels.Audio;
using Voicipher.Domain.Validation;

namespace Voicipher.Business.Commands.Audio
{
    public class UpdateAudioFileCommand : Command<UpdateAudioFileInputModel, CommandResult<FileItemOutputModel>>, IUpdateAudioFileCommand
    {
        private readonly IMessageCenterService _messageCenterService;
        private readonly IAudioFileRepository _audioFileRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public UpdateAudioFileCommand(
            IMessageCenterService messageCenterService,
            IAudioFileRepository audioFileRepository,
            IMapper mapper,
            ILogger logger)
        {
            _messageCenterService = messageCenterService;
            _audioFileRepository = audioFileRepository;
            _mapper = mapper;
            _logger = logger.ForContext<UpdateAudioFileCommand>();
        }

        protected override async Task<CommandResult<FileItemOutputModel>> Execute(UpdateAudioFileInputModel parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var userId = principal.GetNameIdentifier();
            var validationResult = parameter.Validate();
            if (!validationResult.IsValid)
            {
                if (validationResult.Errors.ContainsError(nameof(UpdateAudioFileInputModel.Language), ValidationErrorCodes.NotSupportedLanguage))
                {
                    _logger.Error($"[{userId}] Language {parameter.Language} is not supported");
                    throw new OperationErrorException(ErrorCode.EC200);
                }

                if (validationResult.Errors.ContainsError(nameof(UpdateAudioFileInputModel.Language), ValidationErrorCodes.NotSupportedLanguageModel))
                {
                    _logger.Error($"[{userId}] Language phone call model is not supported");
                    throw new OperationErrorException(ErrorCode.EC203);
                }

                if (validationResult.Errors.ContainsError(nameof(UpdateAudioFileInputModel.EndTime), ValidationErrorCodes.StartTimeGreaterOrEqualThanEndTime))
                {
                    _logger.Error($"[{userId}] Start time for transcription is greater or equal than end time");
                    throw new OperationErrorException(ErrorCode.EC204);
                }

                _logger.Error($"[{userId}] Invalid input data");
                throw new OperationErrorException(ErrorCode.EC600);
            }

            var audioFile = await _audioFileRepository.GetAsync(userId, parameter.AudioFileId, cancellationToken);
            if (audioFile == null)
            {
                _logger.Error($"[{userId}] Audio file {parameter.AudioFileId} was not found");
                throw new OperationErrorException(ErrorCode.EC101);
            }

            if (audioFile.TotalTime < parameter.EndTime)
            {
                _logger.Error($"[{userId}] Transcription end time greater than total time of the audio file");
                throw new OperationErrorException(ErrorCode.EC205);
            }

            audioFile.ApplicationId = parameter.ApplicationId;
            audioFile.Name = parameter.Name;
            audioFile.Language = parameter.Language;
            audioFile.IsPhoneCall = parameter.IsPhoneCall;
            audioFile.TranscriptionStartTime = parameter.StartTime;
            audioFile.TranscriptionEndTime = parameter.EndTime;
            audioFile.DateUpdatedUtc = DateTime.UtcNow;

            await _audioFileRepository.SaveAsync(cancellationToken);
            await _messageCenterService.SendAsync(HubMethodsHelper.GetFilesListChangedMethod(userId));

            var outputModel = _mapper.Map<FileItemOutputModel>(audioFile);
            return new CommandResult<FileItemOutputModel>(outputModel);
        }
    }
}
