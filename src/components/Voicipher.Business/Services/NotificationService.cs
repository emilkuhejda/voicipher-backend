using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Rest;
using Newtonsoft.Json;
using Serilog;
using Voicipher.DataAccess;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Notifications;
using Voicipher.Domain.Settings;

namespace Voicipher.Business.Services
{
    public class NotificationService : INotificationService
    {
        private const string TargetType = "devices_target";
        private const string MediaType = "application/json";

        private readonly IUserDeviceRepository _userDeviceRepository;
        private readonly IInformationMessageRepository _informationMessageRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppSettings _appSettings;
        private readonly ILogger _logger;

        public NotificationService(
            IUserDeviceRepository userDeviceRepository,
            IInformationMessageRepository informationMessageRepository,
            IUnitOfWork unitOfWork,
            IOptions<AppSettings> options,
            ILogger logger)
        {
            _userDeviceRepository = userDeviceRepository;
            _informationMessageRepository = informationMessageRepository;
            _unitOfWork = unitOfWork;
            _appSettings = options.Value;
            _logger = logger.ForContext<NotificationService>();
        }

        public async Task<Dictionary<Language, NotificationResult>> SendAsync(Guid informationMessageId, Guid? userId = null, CancellationToken cancellationToken = default)
        {
            var informationMessage = await _informationMessageRepository.GetAsync(informationMessageId, cancellationToken);
            if (informationMessage == null)
            {
                _logger.Error($"Information message {informationMessageId} not found");
                throw new EntryPointNotFoundException($"Information message {informationMessageId} not found");
            }

            if (!informationMessage.LanguageVersions.Any())
            {
                _logger.Error($"Information message {informationMessage.Id} does not contain any language version");
                throw new LanguageVersionNotExistsException();
            }

            var notificationResults = new Dictionary<Language, NotificationResult>();

            foreach (var languageVersion in informationMessage.LanguageVersions)
            {
                foreach (var runtimePlatform in Enum.GetValues(typeof(RuntimePlatform)).Cast<RuntimePlatform>().Where(x => x != RuntimePlatform.Undefined))
                {
                    _logger.Verbose($"Start sending push notification for platform {runtimePlatform} and in language version {languageVersion.Language}");

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

                        using (var operationResponse = await SendWithHttpMessagesAsync(pushNotification, runtimePlatform, cancellationToken))
                        {
                            _logger.Information($"Notification {informationMessage.Id} was sent to runtime platform {runtimePlatform} for language {languageVersion.Language}");

                            switch (runtimePlatform)
                            {
                                case RuntimePlatform.Android:
                                    languageVersion.SentOnAndroid = true;
                                    break;
                                case RuntimePlatform.Osx:
                                    languageVersion.SentOnOsx = true;
                                    break;
                            }

                            _logger.Verbose($"Update information message sent status for language version {languageVersion.Language} and platform {runtimePlatform} to {true}");

                            notificationResults.Add(languageVersion.Language, operationResponse.Body);
                        }
                    }
                }
            }

            informationMessage.DatePublishedUtc = DateTime.UtcNow;
            await _unitOfWork.SaveAsync(cancellationToken);

            return notificationResults;
        }

        private async Task<HttpOperationResponse<NotificationResult>> SendWithHttpMessagesAsync(PushNotification pushNotification, RuntimePlatform runtimePlatform, CancellationToken cancellationToken)
        {
            var notificationSettings = _appSettings.NotificationSettings;

            HttpClient httpClient = null;
            HttpRequestMessage httpRequest = null;
            HttpResponseMessage httpResponse = null;
            try
            {
                httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaType));
                httpClient.DefaultRequestHeaders.Add(notificationSettings.ApiKeyName, notificationSettings.AccessToken);

                var content = JsonConvert.SerializeObject(pushNotification);
                var applicationName = runtimePlatform == RuntimePlatform.Android
                    ? notificationSettings.AppNameAndroid
                    : notificationSettings.AppNameOsx;
                var url =
                    $"{notificationSettings.BaseUrl}/{notificationSettings.Organization}/{applicationName}/{notificationSettings.Apis}";

                httpRequest = new HttpRequestMessage
                {
                    Method = new HttpMethod("POST"),
                    Content = new StringContent(content, Encoding.UTF8, MediaType),
                    RequestUri = new Uri(url, UriKind.Absolute)
                };

                cancellationToken.ThrowIfCancellationRequested();
                _logger.Verbose($"Send request to url {url}");
                httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
                _logger.Verbose($"Response status code {httpResponse.StatusCode}");
                cancellationToken.ThrowIfCancellationRequested();

                var responseContent = await httpResponse.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                var statusCode = httpResponse.StatusCode;
                if (statusCode != HttpStatusCode.Accepted)
                {
                    var wrapper = JsonConvert.DeserializeObject<NotificationErrorWrapper>(responseContent);
                    throw new NotificationErrorException(wrapper.Error);
                }

                return new HttpOperationResponse<NotificationResult>
                {
                    Body = JsonConvert.DeserializeObject<NotificationResult>(responseContent),
                    Request = httpRequest,
                    Response = httpResponse
                };
            }
            catch (JsonException ex)
            {
                _logger.Error(ex, "Unable to deserialize the response");
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Send notification message failed");
                throw;
            }
            finally
            {
                httpClient?.Dispose();
                httpRequest?.Dispose();
                httpResponse?.Dispose();
            }
        }
    }
}
