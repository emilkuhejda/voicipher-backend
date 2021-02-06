using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.InputModels.Authentication;
using Voicipher.Domain.Interfaces.Commands.Authentication;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.OutputModels.Authentication;

namespace Voicipher.Business.Commands.Authentication
{
    public class UserRegistrationCommand : Command<UserRegistrationInputModel, CommandResult<UserRegistrationOutputModel>>, IUserRegistrationCommand
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserRegistrationCommand(
            IUserRepository userRepository,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        protected override async Task<CommandResult<UserRegistrationOutputModel>> Execute(UserRegistrationInputModel parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            var outputModel = new UserRegistrationOutputModel();

            return new CommandResult<UserRegistrationOutputModel>(outputModel);
        }
    }
}
