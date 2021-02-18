using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Serilog;
using Voicipher.Business.Infrastructure;
using Voicipher.Business.Utils;
using Voicipher.Common.Helpers;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.InputModels.Authentication;
using Voicipher.Domain.Interfaces.Commands.Authentication;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.OutputModels.MetaData;
using Voicipher.Domain.Settings;

namespace Voicipher.Business.Commands.Authentication
{
    public class AuthenticateUserCommand : Command<AuthenticateUserInputModel, CommandResult<AdministratorOutputModel>>, IAuthenticateUserCommand
    {
        private readonly IAdministratorRepository _administratorRepository;
        private readonly AppSettings _appSettings;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public AuthenticateUserCommand(
            IAdministratorRepository administratorRepository,
            IOptions<AppSettings> options,
            IMapper mapper,
            ILogger logger)
        {
            _administratorRepository = administratorRepository;
            _appSettings = options.Value;
            _mapper = mapper;
            _logger = logger.ForContext<AuthenticateUserCommand>();
        }

        protected override async Task<CommandResult<AdministratorOutputModel>> Execute(AuthenticateUserInputModel parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            _logger.Information($"Administrator authentication with user name {parameter.Username}");

            if (!parameter.Validate().IsValid)
            {
                _logger.Error("Invalid input data.");

                throw new OperationErrorException(ErrorCode.EC600);
            }

            var administrator = await _administratorRepository.GetByNameAsync(parameter.Username, cancellationToken);
            if (administrator == null)
            {
                _logger.Warning($"User {parameter.Username} was not found.");

                throw new OperationErrorException(StatusCodes.Status404NotFound);
            }

            if (!PasswordHelper.VerifyPasswordHash(parameter.Password, administrator.PasswordHash, administrator.PasswordSalt))
            {
                _logger.Warning($"Password verification failed for user name {parameter.Username}.");

                throw new OperationErrorException(StatusCodes.Status404NotFound);
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, administrator.Id.ToString()),
                new Claim(ClaimTypes.Role, Role.Admin.ToString())
            };

            var token = TokenHelper.Generate(_appSettings.SecretKey, claims, TimeSpan.FromDays(7));

            _logger.Information($"User authentication for {parameter.Username} was successful");

            var outputModel = _mapper.Map<AdministratorOutputModel>(
                administrator,
                opt => opt.AfterMap((_, o) =>
                {
                    o.Token = token;
                }));

            return new CommandResult<AdministratorOutputModel>(outputModel);
        }
    }
}
