using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Notifications;

namespace Voicipher.Business.Services
{
    public class NotificationService : INotificationService
    {
        public Task<Dictionary<Language, NotificationResult>> SendAsync(Guid informationMessageId, Guid? userId = null, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new Dictionary<Language, NotificationResult>());
        }

        /*public async Task<Dictionary<Language, NotificationResult>> SendAsync(Guid informationMessageId, Guid? userId = null, CancellationToken cancellationToken = default)
        {
            const string targetType = "devices_target";

            var informationMessage = await _informationMessageRepository.GetByIdAsync(informationMessageId, cancellationToken);
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
                                Type = targetType,
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
            const string mediaType = "application/json";

            var notificationSettings = _appSettings.NotificationSettings;

            HttpClient httpClient = null;
            HttpRequestMessage httpRequest = null;
            HttpResponseMessage httpResponse = null;
            try
            {
                httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
                httpClient.DefaultRequestHeaders.Add(notificationSettings.ApiKeyName, notificationSettings.AccessToken);

                var content = JsonConvert.SerializeObject(pushNotification);
                var applicationName = runtimePlatform == RuntimePlatform.Android
                    ? notificationSettings.AppNameAndroid
                    : notificationSettings.AppNameOsx;
                var url = $"{notificationSettings.BaseUrl}/{notificationSettings.Organization}/{applicationName}/{notificationSettings.Apis}";

                httpRequest = new HttpRequestMessage
                {
                    Method = new HttpMethod("POST"),
                    Content = new StringContent(content, Encoding.UTF8, mediaType),
                    RequestUri = new Uri(url, UriKind.Absolute)
                };

                cancellationToken.ThrowIfCancellationRequested();
                _logger.Verbose($"Send request to url {url}");
                httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken);
                _logger.Verbose($"Response status code {httpResponse.StatusCode}");
                cancellationToken.ThrowIfCancellationRequested();

                var responseContent = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
                var statusCode = httpResponse.StatusCode;
                if (statusCode != HttpStatusCode.Accepted)
                {
                    var notificationError = JsonConvert.DeserializeObject<NotificationError>(responseContent);
                    _logger.Error($"Notification error received {JsonConvert.SerializeObject(notificationError)}");
                    throw new NotificationErrorException(notificationError);
                }

                return new HttpOperationResponse<NotificationResult>
                {
                    Body = JsonConvert.DeserializeObject<NotificationResult>(responseContent),
                    Request = httpRequest,
                    Response = httpResponse
                };
            }
            finally
            {
                httpClient?.Dispose();
                httpRequest?.Dispose();
                httpResponse?.Dispose();
            }
        }*/
    }
}
