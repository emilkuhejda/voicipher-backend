using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using Voicipher.Business.Extensions;
using Voicipher.Business.Infrastructure;
using Voicipher.DataAccess;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.InputModels;
using Voicipher.Domain.Interfaces.Commands;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;
using Voicipher.Domain.OutputModels;
using Voicipher.Domain.Payloads;
using Voicipher.Domain.Settings;
using Voicipher.Domain.Validation;

namespace Voicipher.Business.Commands
{
    public class UpdateSpeechResultsCommand : Command<SpeechResultInputModel[], CommandResult<TimeSpanWrapperOutputModel>>, IUpdateSpeechResultsCommand
    {
        private readonly IModifySubscriptionTimeCommand _modifySubscriptionTimeCommand;
        private readonly ISpeechResultRepository _speechResultRepository;
        private readonly ICurrentUserSubscriptionRepository _currentUserSubscriptionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly ILogger _logger;

        public UpdateSpeechResultsCommand(
            IModifySubscriptionTimeCommand modifySubscriptionTimeCommand,
            ISpeechResultRepository speechResultRepository,
            ICurrentUserSubscriptionRepository currentUserSubscriptionRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IOptions<AppSettings> options,
            ILogger logger)
        {
            _modifySubscriptionTimeCommand = modifySubscriptionTimeCommand;
            _speechResultRepository = speechResultRepository;
            _currentUserSubscriptionRepository = currentUserSubscriptionRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _appSettings = options.Value;
            _logger = logger.ForContext<UpdateSpeechResultsCommand>();
        }

        protected override async Task<CommandResult<TimeSpanWrapperOutputModel>> Execute(SpeechResultInputModel[] parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var userId = principal.GetNameIdentifier();

            try
            {
                using (var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken))
                {
                    var speechResults = parameter.Select(_mapper.Map<SpeechResult>).ToArray();
                    _speechResultRepository.UpdateAll(speechResults);
                    await _unitOfWork.SaveAsync(cancellationToken);

                    var totalTime = TimeSpan.FromTicks(parameter.Sum(x => x.Ticks));
                    var modifySubscriptionTimePayload = new ModifySubscriptionTimePayload
                    {
                        UserId = userId,
                        ApplicationId = _appSettings.ApplicationId,
                        Time = totalTime,
                        Operation = SubscriptionOperation.Remove
                    };

                    var commandResult = await _modifySubscriptionTimeCommand.ExecuteAsync(modifySubscriptionTimePayload, principal, cancellationToken);
                    if (!commandResult.IsSuccess)
                    {
                        if (commandResult.Error.ErrorCode == ValidationErrorCodes.NotEnoughSubscriptionTime)
                            throw new OperationErrorException(ErrorCode.EC300);

                        throw new OperationErrorException(ErrorCode.EC603);
                    }

                    _logger.Information($"Update speech results total time. [{userId}]");

                    await transaction.CommitAsync(cancellationToken);
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.Error(ex, "An error occurred while updating the entries");

                throw new OperationErrorException(ErrorCode.EC400);
            }

            var remainingTime = await _currentUserSubscriptionRepository.GetRemainingTimeAsync(userId, cancellationToken);
            return new CommandResult<TimeSpanWrapperOutputModel>(new TimeSpanWrapperOutputModel(remainingTime.Ticks));
        }
    }
}
