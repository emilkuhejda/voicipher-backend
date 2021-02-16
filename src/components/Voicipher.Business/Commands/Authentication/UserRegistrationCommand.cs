using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using Serilog;
using Voicipher.Business.Extensions;
using Voicipher.Business.Infrastructure;
using Voicipher.Business.Utils;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.InputModels.Authentication;
using Voicipher.Domain.Interfaces.Commands.Authentication;
using Voicipher.Domain.Interfaces.Queries.EndUser;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;
using Voicipher.Domain.OutputModels;
using Voicipher.Domain.OutputModels.Authentication;
using Voicipher.Domain.Settings;

namespace Voicipher.Business.Commands.Authentication
{
    public class UserRegistrationCommand : Command<UserRegistrationInputModel, CommandResult<UserRegistrationOutputModel>>, IUserRegistrationCommand
    {
        private readonly IGetUserQuery _getUserQuery;
        private readonly IUserRepository _userRepository;
        private readonly IUserDeviceRepository _userDeviceRepository;
        private readonly AppSettings _appSettings;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public UserRegistrationCommand(
            IGetUserQuery getUserQuery,
            IUserRepository userRepository,
            IUserDeviceRepository userDeviceRepository,
            IOptions<AppSettings> options,
            IMapper mapper,
            ILogger logger)
        {
            _getUserQuery = getUserQuery;
            _userRepository = userRepository;
            _userDeviceRepository = userDeviceRepository;
            _appSettings = options.Value;
            _mapper = mapper;
            _logger = logger.ForContext<UserRegistrationCommand>();
        }

        protected override async Task<CommandResult<UserRegistrationOutputModel>> Execute(UserRegistrationInputModel parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var inputValidationResult = parameter.Validate();
            if (!inputValidationResult.IsValid)
            {
                _logger.Error($"User input is invalid. {inputValidationResult.ToJson()}");

                return new CommandResult<UserRegistrationOutputModel>(inputValidationResult.Errors);
            }

            _logger.Information($"Start user authentication. User email: {parameter.Email}");

            var userQueryResult = await _getUserQuery.ExecuteAsync(parameter.Id, principal, cancellationToken);
            if (!userQueryResult.IsSuccess)
                return new CommandResult<UserRegistrationOutputModel>(userQueryResult.ValidationErrors);

            var user = userQueryResult.Value;
            if (user == null)
            {
                _logger.Information($"User with ID '{parameter.Id}' and email '{parameter.Email}' not exists. Start user registration process");

                user = _mapper.Map<User>(parameter);

                var userValidationResult = user.Validate();
                if (!userValidationResult.IsValid)
                    return new CommandResult<UserRegistrationOutputModel>(userValidationResult.Errors);

                await _userRepository.AddAsync(user);
            }
            else
            {
                if (parameter.Device != null)
                {
                    _logger.Information($"User with ID '{parameter.Id}' and email '{parameter.Email}' already exists. Update device information");

                    var userDevice = _mapper.Map<UserDevice>(parameter);
                    await _userDeviceRepository.AddOrUpdateAsync(userDevice, cancellationToken);
                }
            }

            var (token, refreshToken) = GenerateTokens(user);
            var outputModel = new UserRegistrationOutputModel
            {
                Token = token,
                RefreshToken = refreshToken,
                Identity = _mapper.Map<IdentityOutputModel>(user),
                RemainingTime = new TimeSpanWrapperOutputModel(user.CurrentUserSubscription.Ticks)
            };

            var outputValidationResult = outputModel.Validate();
            if (!outputValidationResult.IsValid)
            {
                _logger.Error("Invalid output model.");

                return new CommandResult<UserRegistrationOutputModel>(outputValidationResult.Errors);
            }

            await _userRepository.SaveAsync(cancellationToken);

            _logger.Information($"User '{parameter.Email}' was successfully registered and token was created");

            return new CommandResult<UserRegistrationOutputModel>(outputModel);
        }

        private (string token, string refreshToken) GenerateTokens(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, nameof(Role.User))
            };

            var token = TokenHelper.Generate(_appSettings.SecretKey, claims, TimeSpan.FromDays(180));

            var refreshClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, nameof(Role.Security))
            };

            var refreshToken = TokenHelper.Generate(_appSettings.SecretKey, refreshClaims, TimeSpan.FromDays(730));

            return (token, refreshToken);
        }
    }
}
