using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Infrastructure;
using Voicipher.Domain.OutputModels.MetaData;
using Voicipher.Domain.Payloads;

namespace Voicipher.Domain.Interfaces.Commands.ControlPanel
{
    public interface IGenerateTokenCommand : ICommand<GenerateTokenPayload, CommandResult<AdministratorTokenOutputModel>>
    {
    }
}
