using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Serilog;
using Voicipher.Business.Extensions;
using Voicipher.Business.Infrastructure;
using Voicipher.Business.Utils;
using Voicipher.Common.Helpers;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.InputModels.MetaData;
using Voicipher.Domain.Interfaces.Commands.ControlPanel;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.OutputModels.MetaData;
using Voicipher.Domain.Settings;
using Voicipher.Domain.Validation;

namespace Voicipher.Business.Commands.ControlPanel
{
    public class GenerateTokenCommand : Command<CreateTokenInputModel, CommandResult<AdministratorTokenOutputModel>>, IGenerateTokenCommand
    {
        private readonly IAdministratorRepository _administratorRepository;
        private readonly AppSettings _appSettings;
        private readonly ILogger _logger;

        public GenerateTokenCommand(
            IAdministratorRepository administratorRepository,
            IOptions<AppSettings> options,
            ILogger logger)
        {
            _administratorRepository = administratorRepository;
            _appSettings = options.Value;
            _logger = logger.ForContext<GenerateTokenCommand>();
        }

        protected override async Task<CommandResult<AdministratorTokenOutputModel>> Execute(CreateTokenInputModel parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var validationResult = parameter.Validate();
            if (!validationResult.IsValid)
            {
                _logger.Error($"User input is invalid. {validationResult.ToJson()}");

                return new CommandResult<AdministratorTokenOutputModel>(validationResult.Errors);
            }

            var administrator = await _administratorRepository.GetByNameAsync(parameter.Username, cancellationToken);
            if (administrator == null)
            {
                _logger.Error($"Administrator {parameter.Username} was not found");

                return new CommandResult<AdministratorTokenOutputModel>(new OperationError(ValidationErrorCodes.AdministratorNotFound));
            }

            if (!PasswordHelper.VerifyPasswordHash(parameter.Password, administrator.PasswordHash, administrator.PasswordSalt))
            {
                _logger.Error($"Password verification failed for administrator {parameter.Username}");

                return new CommandResult<AdministratorTokenOutputModel>(new OperationError(ValidationErrorCodes.InvalidPassword));
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, parameter.UserId.ToString()),
                new Claim(ClaimTypes.Role, parameter.Role.ToString())
            };

            var token = TokenHelper.Generate(_appSettings.SecretKey, claims, TimeSpan.FromDays(180));

            _logger.Information($"Token was created for user ID = {parameter.UserId}");

            return new CommandResult<AdministratorTokenOutputModel>(new AdministratorTokenOutputModel(token));
        }
    }
}
