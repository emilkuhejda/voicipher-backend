using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Infrastructure;
using Voicipher.Domain.OutputModels.ControlPanel;
using Voicipher.Domain.Payloads.ControlPanel;

namespace Voicipher.Domain.Interfaces.Queries.ControlPanel
{
    public interface IGetInternalValueQuery<T> : IQuery<GetInternalValuePayload<T>, QueryResult<InternalValueOutputModel<T>>>
    {
    }
}
