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

        public GetLastUpdatesQuery(IAudioFileRepository audioFileRepository)
        {
            _audioFileRepository = audioFileRepository;
        }

        protected override async Task<QueryResult<LastUpdatesOutputModel>> Execute(ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var fileItemUtc = await _audioFileRepository.GetLastUpdateAsync(principal.GetNameIdentifier(), cancellationToken);

            var lastUpdatesOutputModel = new LastUpdatesOutputModel
            {
                FileItemUtc = fileItemUtc,
                DeletedFileItemUtc = DateTime.MinValue,
                TranscribeItemUtc = DateTime.MinValue,
                UserSubscriptionUtc = DateTime.MinValue,
                InformationMessageUtc = DateTime.MinValue
            };

            return new QueryResult<LastUpdatesOutputModel>(lastUpdatesOutputModel);
        }
    }
}
