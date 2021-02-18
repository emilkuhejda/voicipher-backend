using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Serilog;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Commands.Job;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;
using Voicipher.Domain.Payloads.Job;

namespace Voicipher.Business.Commands.Job
{
    public class CreateBackgroundJobCommand : Command<CreateBackgroundJobPayload, CommandResult<BackgroundJobPayload>>, ICreateBackgroundJobCommand
    {
        private readonly IBackgroundJobRepository _backgroundJobRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CreateBackgroundJobCommand(
            IBackgroundJobRepository backgroundJobRepository,
            IMapper mapper,
            ILogger logger)
        {
            _backgroundJobRepository = backgroundJobRepository;
            _mapper = mapper;
            _logger = logger.ForContext<CreateBackgroundJobCommand>();
        }

        protected override async Task<CommandResult<BackgroundJobPayload>> Execute(CreateBackgroundJobPayload parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var backgroundJob = _mapper.Map<BackgroundJob>(parameter);
            var backgroundJobToRestart = await _backgroundJobRepository.GetAsync(backgroundJob.Id, cancellationToken);
            if (backgroundJobToRestart != null)
            {
                _logger.Information($"Background job {backgroundJob.Id} was restored from database");

                var payload = _mapper.Map<BackgroundJobPayload>(backgroundJobToRestart);
                return new CommandResult<BackgroundJobPayload>(payload);
            }

            await _backgroundJobRepository.AddAsync(backgroundJob);
            await _backgroundJobRepository.SaveAsync(cancellationToken);

            _logger.Information($"Background job {backgroundJob.Id} was created");

            var backgroundJobPayload = _mapper.Map<BackgroundJobPayload>(backgroundJob);
            return new CommandResult<BackgroundJobPayload>(backgroundJobPayload);
        }
    }
}
