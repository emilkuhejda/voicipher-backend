using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Rest;
using Serilog;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Models;
using Voicipher.Domain.Notifications;
using Voicipher.Domain.Settings;

namespace Voicipher.Business.Services
{
    public class NotificationsService : INotificationsService
    {
        private const string TargetType = "devices_target";
        private const string MediaType = "application/json";

        private readonly IUserDeviceRepository _userDeviceRepository;
        private readonly IInformationMessageRepository _informationMessageRepository;
        private readonly AppSettings _appSettings;
        private readonly ILogger _logger;

        public NotificationsService(
            IUserDeviceRepository userDeviceRepository,
            IInformationMessageRepository informationMessageRepository,
            IOptions<AppSettings> options,
            ILogger logger)
        {
            _userDeviceRepository = userDeviceRepository;
            _informationMessageRepository = informationMessageRepository;
            _appSettings = options.Value;
            _logger = logger.ForContext<NotificationsService>();
        }

        public async Task<NotificationResult> SendAsync(InformationMessage informationMessage, Guid? userId = null, CancellationToken cancellationToken = default)
        {
            if (!informationMessage.LanguageVersions.Any())
            {
                _logger.Error($"Information message {informationMessage.Id} does not contain language version");
                throw new LanguageVersionNotExistsException();
            }

            foreach (var languageVersion in informationMessage.LanguageVersions)
            {
                foreach (var runtimePlatform in Enum.GetValues(typeof(RuntimePlatform)).Cast<RuntimePlatform>().Where(x => x != RuntimePlatform.Undefined))
                {
                    NotificationResult notificationResult = null;
                    var installationIds = await _userDeviceRepository.GetPlatformSpecificInstallationIdsAsync(runtimePlatform, languageVersion.Language, userId, cancellationToken);
                    if (installationIds.Any())
                    {
                        var pushNotification = new PushNotification
                        {
                            Target = new NotificationTarget
                            {
                                Type = TargetType,
                                Devices = installationIds
                            },
                            Content = new NotificationContent
                            {
                                Name = informationMessage.CampaignName,
                                Title = languageVersion.Title,
                                Body = languageVersion.Message
                            }
                        };
                    }
                }
            }

            await Task.CompletedTask;
            return new NotificationResult();
        }

        private async Task<HttpOperationResponse<NotificationResult>> SendWithHttpMessagesAsync(PushNotification pushNotification, RuntimePlatform runtimePlatform, CancellationToken cancellationToken)
        { }
    }
}
