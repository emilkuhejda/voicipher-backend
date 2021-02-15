using System;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Infrastructure;

namespace Voicipher.Domain.Interfaces.Queries.TranscribeItems
{
    public interface IGetTranscribeItemSourceQuery : IQuery<Guid, QueryResult<byte[]>>
    {
    }
}
