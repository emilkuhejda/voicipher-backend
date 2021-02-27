using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Infrastructure;
using Voicipher.Domain.OutputModels.ControlPanel;
using Voicipher.Domain.Payloads.ControlPanel;

namespace Voicipher.Domain.Interfaces.Queries.ControlPanel
{
    public interface IGetInternalValueQuery<T> : IQuery<InternalValuePayload<T>, QueryResult<InternalValueOutputModel<T>>>
    {
    }
}
