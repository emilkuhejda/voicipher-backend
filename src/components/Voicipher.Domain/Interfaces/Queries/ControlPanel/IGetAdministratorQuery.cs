using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.InputModels.MetaData;
using Voicipher.Domain.Interfaces.Infrastructure;
using Voicipher.Domain.Models;

namespace Voicipher.Domain.Interfaces.Queries.ControlPanel
{
    public interface IGetAdministratorQuery : IQuery<CreateTokenInputModel, QueryResult<Administrator>>
    {
    }
}
