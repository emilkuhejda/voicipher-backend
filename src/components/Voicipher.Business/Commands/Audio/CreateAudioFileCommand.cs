using System;
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
using Voicipher.Domain.InputModels.Audio;
using Voicipher.Domain.Interfaces.Commands.Audio;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;
using Voicipher.Domain.OutputModels.Audio;
using Voicipher.Domain.Utils;

namespace Voicipher.Business.Commands.Audio
{
    public class CreateAudioFileCommand : Command<CreateAudioFileInputModel, CommandResult<AudioFileOutputModel>>, ICreateAudioFileCommand
    {
        private readonly IAudioFileRepository _audioFileRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CreateAudioFileCommand(
            IAudioFileRepository audioFileRepository,
            IMapper mapper,
            ILogger logger)
        {
            _audioFileRepository = audioFileRepository;
            _mapper = mapper;
            _logger = logger.ForContext<CreateAudioFileCommand>();
        }

        protected override async Task<CommandResult<AudioFileOutputModel>> Execute(CreateAudioFileInputModel parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(parameter.Language) && !SupportedLanguages.IsSupported(parameter.Language))
            {
                _logger.Error($"Language '{parameter.Language}' is not supported.");

                throw new OperationErrorException(ErrorCode.EC200);
            }

            var audioFile = new AudioFile
            {
                Id = Guid.NewGuid(),
                UserId = principal.GetNameIdentifier(),
                ApplicationId = parameter.ApplicationId,
                Name = parameter.Name,
                FileName = parameter.FileName,
                Language = parameter.Language,
                Storage = StorageSetting.Azure,
                DateCreated = parameter.DateCreated,
                DateUpdatedUtc = DateTime.UtcNow
            };

            await _audioFileRepository.AddAsync(audioFile);
            await _audioFileRepository.SaveAsync(cancellationToken).ConfigureAwait(false);

            _logger.Information($"File item '{audioFile.Id}' was created.");

            return new CommandResult<AudioFileOutputModel>(_mapper.Map<AudioFileOutputModel>(audioFile));
        }
    }
}
