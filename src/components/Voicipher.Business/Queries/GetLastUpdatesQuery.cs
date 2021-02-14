using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Voicipher.Business.Extensions;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Queries;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Payloads;

namespace Voicipher.Business.Queries
{
    public class GetLastUpdatesQuery : Query<QueryResult<LastUpdatesOutputModel>>, IGetLastUpdatesQuery
    {
        private readonly IAudioFileRepository _audioFileRepository;
        private readonly ITranscribeItemRepository _transcribeItemRepository;
        private readonly IUserSubscriptionRepository _userSubscriptionRepository;
        private readonly IInformationMessageRepository _informationMessageRepository;

        public GetLastUpdatesQuery(
            IAudioFileRepository audioFileRepository,
            ITranscribeItemRepository transcribeItemRepository,
            IUserSubscriptionRepository userSubscriptionRepository,
            IInformationMessageRepository informationMessageRepository)
        {
            _audioFileRepository = audioFileRepository;
            _transcribeItemRepository = transcribeItemRepository;
            _userSubscriptionRepository = userSubscriptionRepository;
            _informationMessageRepository = informationMessageRepository;
        }

        protected override async Task<QueryResult<LastUpdatesOutputModel>> Execute(ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var userId = principal.GetNameIdentifier();
            var fileItemUtc = await _audioFileRepository.GetLastUpdateAsync(userId, cancellationToken);
            var deletedFileItemUtc = await _audioFileRepository.GetDeletedLastUpdateAsync(userId, cancellationToken);
            var transcribeItemUtc = await _transcribeItemRepository.GetLastUpdateAsync(userId, cancellationToken);
            var userSubscriptionUtc = await _userSubscriptionRepository.GetLastUpdateAsync(userId, cancellationToken);
            var informationMessageUtc = await _informationMessageRepository.GetLastUpdateAsync(userId, cancellationToken);

            var lastUpdatesOutputModel = new LastUpdatesOutputModel
            {
                FileItemUtc = fileItemUtc,
                DeletedFileItemUtc = deletedFileItemUtc,
                TranscribeItemUtc = transcribeItemUtc,
                UserSubscriptionUtc = userSubscriptionUtc,
                InformationMessageUtc = informationMessageUtc
            };

            return new QueryResult<LastUpdatesOutputModel>(lastUpdatesOutputModel);
        }
    }
}
