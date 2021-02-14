using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.InputModels.MetaData;
using Voicipher.Domain.Interfaces.Infrastructure;
using Voicipher.Domain.OutputModels.MetaData;

namespace Voicipher.Domain.Interfaces.Commands.ControlPanel
{
    public interface IGenerateTokenCommand : ICommand<CreateTokenInputModel, CommandResult<AdministratorTokenOutputModel>>
    {
    }
}
