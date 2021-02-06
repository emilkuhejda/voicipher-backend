using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Serilog;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.InputModels.Authentication;
using Voicipher.Domain.Interfaces.Commands.Authentication;
using Voicipher.Domain.Interfaces.Queries;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.OutputModels.Authentication;

namespace Voicipher.Business.Commands.Authentication
{
    public class UserRegistrationCommand : Command<UserRegistrationInputModel, CommandResult<UserRegistrationOutputModel>>, IUserRegistrationCommand
    {
        private readonly IGetUserQuery _getUserQuery;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public UserRegistrationCommand(
            IGetUserQuery getUserQuery,
            IUserRepository userRepository,
            IMapper mapper,
            ILogger logger)
        {
            _getUserQuery = getUserQuery;
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger.ForContext<UserRegistrationCommand>();
        }

        protected override async Task<CommandResult<UserRegistrationOutputModel>> Execute(UserRegistrationInputModel parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            _logger.Information($"Start user authentication. User email: '{parameter.Email}'.");

            var userQueryResult = await _getUserQuery.ExecuteAsync(parameter.Id, principal, cancellationToken);
            if (!userQueryResult.IsSuccess)
                return new CommandResult<UserRegistrationOutputModel>(userQueryResult.ValidationErrors);

            var outputModel = new UserRegistrationOutputModel();

            return new CommandResult<UserRegistrationOutputModel>(outputModel);
        }
    }
}
