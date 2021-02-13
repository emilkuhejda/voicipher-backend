using System;
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

        public GetLastUpdatesQuery(
            IAudioFileRepository audioFileRepository,
            ITranscribeItemRepository transcribeItemRepository,
            IUserSubscriptionRepository userSubscriptionRepository)
        {
            _audioFileRepository = audioFileRepository;
            _transcribeItemRepository = transcribeItemRepository;
            _userSubscriptionRepository = userSubscriptionRepository;
        }

        protected override async Task<QueryResult<LastUpdatesOutputModel>> Execute(ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var userId = principal.GetNameIdentifier();
            var fileItemUtc = await _audioFileRepository.GetLastUpdateAsync(userId, cancellationToken);
            var transcribeItemUtc = await _transcribeItemRepository.GetLastUpdateAsync(userId, cancellationToken);
            var userSubscriptionUtc = await _userSubscriptionRepository.GetLastUpdateAsync(userId, cancellationToken);

            var lastUpdatesOutputModel = new LastUpdatesOutputModel
            {
                FileItemUtc = fileItemUtc,
                DeletedFileItemUtc = DateTime.MinValue,
                TranscribeItemUtc = transcribeItemUtc,
                UserSubscriptionUtc = userSubscriptionUtc,
                InformationMessageUtc = DateTime.MinValue
            };

            return new QueryResult<LastUpdatesOutputModel>(lastUpdatesOutputModel);
        }
    }
}
