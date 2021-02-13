using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Voicipher.Business.Extensions;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Queries.Audio;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.OutputModels.Audio;
using Voicipher.Domain.Payloads.Audio;

namespace Voicipher.Business.Queries.Audio
{
    public class GetAudioFilesQuery : Query<AudioFilesPayload, QueryResult<AudioFileOutputModel[]>>, IGetAudioFilesQuery
    {
        private readonly IAudioFileRepository _audioFileRepository;
        private readonly IMapper _mapper;

        public GetAudioFilesQuery(IAudioFileRepository audioFileRepository, IMapper mapper)
        {
            _audioFileRepository = audioFileRepository;
            _mapper = mapper;
        }

        protected override async Task<QueryResult<AudioFileOutputModel[]>> Execute(AudioFilesPayload parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var userId = principal.GetNameIdentifier();
            var audioFiles = await _audioFileRepository.GetAllAsync(userId, parameter.UpdatedAfter, parameter.ApplicationId, cancellationToken);

            var outputModels = audioFiles.Select(x => _mapper.Map<AudioFileOutputModel>(x)).ToArray();
            return new QueryResult<AudioFileOutputModel[]>(outputModels);
        }
    }
}
