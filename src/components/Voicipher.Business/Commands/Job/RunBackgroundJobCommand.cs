using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Voicipher.Business.Extensions;
using Voicipher.Business.Infrastructure;
using Voicipher.Business.Utils;
using Voicipher.DataAccess;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Interfaces.StateMachine;
using Voicipher.Domain.Payloads.Job;

namespace Voicipher.Business.Commands.Job
{
    public class RunBackgroundJobCommand : Command<BackgroundJobPayload, CommandResult>, IRunBackgroundJobCommand
    {
        private readonly IJobStateMachine _jobStateMachine;
        private readonly IMessageCenterService _messageCenterService;
        private readonly IBackgroundJobRepository _backgroundJobRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public RunBackgroundJobCommand(
            IJobStateMachine jobStateMachine,
            IMessageCenterService messageCenterService,
            IBackgroundJobRepository backgroundJobRepository,
            IUnitOfWork unitOfWork,
            ILogger logger)
        {
            _jobStateMachine = jobStateMachine;
            _messageCenterService = messageCenterService;
            _backgroundJobRepository = backgroundJobRepository;
            _unitOfWork = unitOfWork;
            _logger = logger.ForContext<RunBackgroundJobCommand>();
        }

        protected override async Task<CommandResult> Execute(BackgroundJobPayload parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            _logger.Information($"Background job {parameter.Id} has started");

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
                    await _jobStateMachine.DoCompleteAsync(cancellationToken);

                    _logger.Information($"Background job {parameter.Id} is completed");

                    return new CommandResult();
                }
                catch (Exception)
                {
                    var fileName = parameter.GetParameter(BackgroundJobParameter.FileName, string.Empty);
                    if (!string.IsNullOrWhiteSpace(fileName))
                    {
                        await _messageCenterService.SendAsync(HubMethodsHelper.GetRecognitionErrorMethod(parameter.UserId), fileName);
                    }

                    throw;
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
