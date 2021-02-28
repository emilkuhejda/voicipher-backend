using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Serilog;
using Voicipher.Domain.Enums;
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

        public Task<NotificationResult> SendAsync(InformationMessage informationMessage, RuntimePlatform runtimePlatform, Language language, Guid? userId = null, CancellationToken cancellationToken = default)
        {
        }
    }
}
