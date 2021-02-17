using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Serilog;
using Voicipher.Business.Infrastructure;
using Voicipher.DataAccess;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Interfaces.StateMachine;
using Voicipher.Domain.Models;
using Voicipher.Domain.Payloads.Job;

namespace Voicipher.Business.Commands.Job
{
    public class RunBackgroundJobCommand : Command<BackgroundJobPayload, CommandResult>, IRunBackgroundJobCommand
    {
        private readonly IJobStateMachine _jobStateMachine;
        private readonly IBackgroundJobRepository _backgroundJobRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public RunBackgroundJobCommand(
            IJobStateMachine jobStateMachine,
            IBackgroundJobRepository backgroundJobRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger logger)
        {
            _jobStateMachine = jobStateMachine;
            _backgroundJobRepository = backgroundJobRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger.ForContext<RunBackgroundJobCommand>();
        }

        protected override async Task<CommandResult> Execute(BackgroundJobPayload parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            _logger.Information($"Background job {parameter.Id} has started");

            var recognitionFile = _mapper.Map<RecognitionFile>(parameter);

            using (var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken))
            {
                try
                {
                    var backgroundJob = await _backgroundJobRepository.GetAsync(parameter.Id, cancellationToken);
                    if (backgroundJob == null)
                        throw new InvalidOperationException($"Background job {parameter.Id} not found");

                    _jobStateMachine.DoInit(backgroundJob);
                    await _jobStateMachine.DoValidationAsync(cancellationToken);
                    await _jobStateMachine.DoConvertingAsync(cancellationToken);
                    await _jobStateMachine.DoProcessingAsync(cancellationToken);
                    _jobStateMachine.DoCompleteAsync(cancellationToken);

                    _logger.Information($"Background job {parameter.Id} is completed");

                    return new CommandResult();
                }
                finally
                {
                    await _jobStateMachine.SaveAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);
                }
            }
        }
    }
}
