using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.InputModels.MetaData;
using Voicipher.Domain.Interfaces.Infrastructure;
using Voicipher.Domain.OutputModels.MetaData;

namespace Voicipher.Domain.Interfaces.Queries.ControlPanel
{
    public interface IGetAdministratorQuery : IQuery<CreateTokenInputModel, QueryResult<AdministratorTokenOutputModel>>
    {
    }
}
