using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Serilog;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.InputModels.Authentication;
using Voicipher.Domain.Interfaces.Commands.Authentication;
using Voicipher.Domain.Interfaces.Queries.EndUser;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;
using Voicipher.Domain.OutputModels.Authentication;

namespace Voicipher.Business.Commands.Authentication
{
    public class UserRegistrationCommand : Command<UserRegistrationInputModel, CommandResult<UserRegistrationOutputModel>>, IUserRegistrationCommand
    {
        private readonly IGetUserQuery _getUserQuery;
        private readonly IUserRepository _userRepository;
        private readonly IUserDeviceRepository _userDeviceRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public UserRegistrationCommand(
            IGetUserQuery getUserQuery,
            IUserRepository userRepository,
            IUserDeviceRepository userDeviceRepository,
            IMapper mapper,
            ILogger logger)
        {
            _getUserQuery = getUserQuery;
            _userRepository = userRepository;
            _userDeviceRepository = userDeviceRepository;
            _mapper = mapper;
            _logger = logger.ForContext<UserRegistrationCommand>();
        }

        protected override async Task<CommandResult<UserRegistrationOutputModel>> Execute(UserRegistrationInputModel parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            _logger.Information($"Start user authentication. User email: '{parameter.Email}'.");

            var userQueryResult = await _getUserQuery.ExecuteAsync(parameter.Id, principal, cancellationToken);
            if (!userQueryResult.IsSuccess)
                return new CommandResult<UserRegistrationOutputModel>(userQueryResult.ValidationErrors);

            var user = userQueryResult.Value;
            if (user == null)
            {
                _logger.Information($"User with ID '{parameter.Id}' and email '{parameter.Email}' not exists. Start user registration process.");

                user = _mapper.Map<User>(parameter);

                var validationResult = user.Validate();
                if (!validationResult.IsValid)
                    return new CommandResult<UserRegistrationOutputModel>(validationResult.Errors);

                await _userRepository.AddAsync(user);
            }
            else
            {
                _logger.Information($"User with ID '{parameter.Id}' and email '{parameter.Email}' already exists. Update device information.");

                var userDevice = _mapper.Map<UserDevice>(parameter);
                await _userDeviceRepository.AddOrUpdateAsync(userDevice, cancellationToken);
            }

            await _userRepository.SaveAsync(cancellationToken);

            var outputModel = new UserRegistrationOutputModel();

            return new CommandResult<UserRegistrationOutputModel>(outputModel);
        }
    }
}
