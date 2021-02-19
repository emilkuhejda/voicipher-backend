using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Serilog;
using Voicipher.Business.Extensions;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Commands;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;
using Voicipher.Domain.OutputModels;
using Voicipher.Domain.Settings;

namespace Voicipher.Business.Commands
{
    public class CreateSpeechConfigurationCommand : Command<Guid, CommandResult<SpeechConfigurationOutputModel>>, ICreateSpeechConfigurationCommand
    {
        private readonly IRecognizedAudioSampleRepository _recognizedAudioSampleRepository;
        private readonly ICurrentUserSubscriptionRepository _currentUserSubscriptionRepository;
        private readonly AppSettings _appSettings;
        private readonly ILogger _logger;

        public CreateSpeechConfigurationCommand(
            IRecognizedAudioSampleRepository recognizedAudioSampleRepository,
            ICurrentUserSubscriptionRepository currentUserSubscriptionRepository,
            IOptions<AppSettings> options,
            ILogger logger)
        {
            _recognizedAudioSampleRepository = recognizedAudioSampleRepository;
            _currentUserSubscriptionRepository = currentUserSubscriptionRepository;
            _appSettings = options.Value;
            _logger = logger.ForContext<CreateSpeechConfigurationCommand>();
        }

        protected override async Task<CommandResult<SpeechConfigurationOutputModel>> Execute(Guid parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var userId = principal.GetNameIdentifier();
            var recognizedAudioSample = new RecognizedAudioSample
            {
                Id = parameter,
                UserId = userId,
                DateCreatedUtc = DateTime.UtcNow
            };

            await _recognizedAudioSampleRepository.AddAsync(recognizedAudioSample);
            await _currentUserSubscriptionRepository.SaveAsync(cancellationToken);

            var remainingTime = await _currentUserSubscriptionRepository.GetRemainingTimeAsync(userId, cancellationToken);
            var outputModel = new SpeechConfigurationOutputModel
            {
                SubscriptionKey = _appSettings.AzureSpeechConfiguration.SubscriptionKey,
                SpeechRegion = _appSettings.AzureSpeechConfiguration.Region,
                AudioSampleId = recognizedAudioSample.Id,
                SubscriptionRemainingTimeTicks = remainingTime.Ticks
            };

            _logger.Information($"[{userId}] User retrieved speech recognition configuration");

            return new CommandResult<SpeechConfigurationOutputModel>(outputModel);
        }
    }
}
