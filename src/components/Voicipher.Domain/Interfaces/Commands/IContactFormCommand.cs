using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.InputModels;
using Voicipher.Domain.Interfaces.Infrastructure;
using Voicipher.Domain.OutputModels;

namespace Voicipher.Domain.Interfaces.Commands
{
    public interface IContactFormCommand : ICommand<ContactFormInputModel, CommandResult<OkOutputModel>>
    {
    }
}
