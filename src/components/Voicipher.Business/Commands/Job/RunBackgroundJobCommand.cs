using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Serilog;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Channels;
using Voicipher.Domain.Models;
using Voicipher.Domain.Payloads.Job;

namespace Voicipher.Business.Commands.Job
{
    public class RunBackgroundJobCommand : Command<BackgroundJobPayload, CommandResult>, IRunBackgroundJobCommand
    {
        private readonly IAudioFileProcessingChannel _audioFileProcessingChannel;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public RunBackgroundJobCommand(
            IAudioFileProcessingChannel audioFileProcessingChannel,
            IMapper mapper,
            ILogger logger)
        {
            _audioFileProcessingChannel = audioFileProcessingChannel;
            _mapper = mapper;
            _logger = logger.ForContext<RunBackgroundJobCommand>();
        }

        protected override async Task<CommandResult> Execute(BackgroundJobPayload parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            _logger.Information($"Background job ID = {parameter.Id} has started");

            var recognitionFile = _mapper.Map<RecognitionFile>(parameter);
            _audioFileProcessingChannel.FinishProcessing(recognitionFile);

            return new CommandResult();
        }
    }
}
