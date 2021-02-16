using System.IO;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Serilog;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Channels;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;
using Voicipher.Domain.Payloads.Job;

namespace Voicipher.Business.Commands.Job
{
    public class RunBackgroundJobCommand : Command<BackgroundJobPayload, CommandResult>, IRunBackgroundJobCommand
    {
        private readonly IAudioFileProcessingChannel _audioFileProcessingChannel;
        private readonly IAudioFileRepository _audioFileRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public RunBackgroundJobCommand(
            IAudioFileProcessingChannel audioFileProcessingChannel,
            IAudioFileRepository audioFileRepository,
            IMapper mapper,
            ILogger logger)
        {
            _audioFileProcessingChannel = audioFileProcessingChannel;
            _audioFileRepository = audioFileRepository;
            _mapper = mapper;
            _logger = logger.ForContext<RunBackgroundJobCommand>();
        }

        protected override async Task<CommandResult> Execute(BackgroundJobPayload parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            _logger.Information($"Background job ID {parameter.Id} has started");

            var recognitionFile = _mapper.Map<RecognitionFile>(parameter);

            try
            {
                var audioFile = await _audioFileRepository.GetAsync(parameter.UserId, parameter.AudioFileId, cancellationToken);
                if (audioFile == null)
                {
                    throw new FileNotFoundException($"Audio file {parameter.AudioFileId} not found");
                }

                return new CommandResult();
            }
            finally
            {
                _audioFileProcessingChannel.FinishProcessing(recognitionFile);
            }
        }
    }
}
