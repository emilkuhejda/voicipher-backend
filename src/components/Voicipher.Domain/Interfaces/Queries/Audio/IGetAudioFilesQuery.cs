using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Infrastructure;
using Voicipher.Domain.OutputModels.Audio;
using Voicipher.Domain.Payloads.Audio;

namespace Voicipher.Domain.Interfaces.Queries.Audio
{
    public interface IGetAudioFilesQuery : IQuery<AudioFilesPayload, QueryResult<AudioFileOutputModel[]>>
    {
    }
}
