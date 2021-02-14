using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.InputModels;
using Voicipher.Domain.Interfaces.Commands;
using Voicipher.Domain.OutputModels;

namespace Voicipher.Business.Commands
{
    public class ContactFormCommand : Command<ContactFormInputModel, CommandResult<OkOutputModel>>, IContactFormCommand
    {
        protected override Task<CommandResult<OkOutputModel>> Execute(ContactFormInputModel parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
