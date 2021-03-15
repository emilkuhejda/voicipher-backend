using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Voicipher.Business.Extensions;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Queries;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Payloads;
using Voicipher.Domain.Settings;

namespace Voicipher.Business.Queries
{
    public class GetLastUpdatesQuery : Query<QueryResult<LastUpdatesOutputModel>>, IGetLastUpdatesQuery
    {
        private readonly IAudioFileRepository _audioFileRepository;
        private readonly ITranscribeItemRepository _transcribeItemRepository;
        private readonly ICurrentUserSubscriptionRepository _currentUserSubscriptionRepository;
        private readonly IInformationMessageRepository _informationMessageRepository;
        private readonly AppSettings _appSettings;

        public GetLastUpdatesQuery(
            IAudioFileRepository audioFileRepository,
            ITranscribeItemRepository transcribeItemRepository,
            ICurrentUserSubscriptionRepository currentUserSubscriptionRepository,
            IInformationMessageRepository informationMessageRepository,
            IOptions<AppSettings> options)
        {
            _audioFileRepository = audioFileRepository;
            _transcribeItemRepository = transcribeItemRepository;
            _currentUserSubscriptionRepository = currentUserSubscriptionRepository;
            _informationMessageRepository = informationMessageRepository;
            _appSettings = options.Value;
        }

        protected override async Task<QueryResult<LastUpdatesOutputModel>> Execute(ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var userId = principal.GetNameIdentifier();
            var fileItemUtc = await _audioFileRepository.GetLastUpdateAsync(userId, cancellationToken);
            var deletedFileItemUtc = await _audioFileRepository.GetDeletedLastUpdateAsync(userId, cancellationToken);
            var transcribeItemUtc = await _transcribeItemRepository.GetLastUpdateAsync(userId, cancellationToken);
            var userSubscriptionUtc = await _currentUserSubscriptionRepository.GetLastUpdateAsync(userId, cancellationToken);
            var informationMessageUtc = await _informationMessageRepository.GetLastUpdateAsync(userId, cancellationToken);

            var lastUpdatesOutputModel = new LastUpdatesOutputModel
            {
                FileItemUtc = fileItemUtc,
                DeletedFileItemUtc = deletedFileItemUtc,
                TranscribeItemUtc = transcribeItemUtc,
                UserSubscriptionUtc = userSubscriptionUtc,
                InformationMessageUtc = informationMessageUtc,
                ApiUrl = _appSettings.ApiUrl,
                ApiVersion = _appSettings.ApiVersion
            };

            return new QueryResult<LastUpdatesOutputModel>(lastUpdatesOutputModel);
        }
    }
}
