using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Infrastructure;
using Voicipher.Domain.Payloads;

namespace Voicipher.Domain.Interfaces.Queries
{
    public interface IGetLastUpdatesQuery : IQuery<QueryResult<LastUpdatesOutputModel>>
    {
    }
}
