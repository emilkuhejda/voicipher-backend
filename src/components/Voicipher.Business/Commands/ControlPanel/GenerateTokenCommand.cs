using System;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Serilog;
using Voicipher.Business.Extensions;
using Voicipher.Business.Infrastructure;
using Voicipher.Business.Utils;
using Voicipher.Domain.Enums;
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
                _logger.Error($"Administrator '{parameter.Username}' was not found.");

                return new CommandResult<AdministratorTokenOutputModel>(new OperationError(ValidationErrorCodes.NotFound));
            }

            validationResult = administrator.Validate();
            if (!validationResult.IsValid)
            {
                _logger.Error($"Administrator '{parameter.Username}' has invalid stored properties.");

                return new CommandResult<AdministratorTokenOutputModel>(new OperationError(ValidationErrorCodes.InvalidStoredValues), validationResult.Errors);
            }

            if (!VerifyPasswordHash(parameter.Password, administrator.PasswordHash, administrator.PasswordSalt))
            {
                _logger.Error($"Password verification failed for administrator '{parameter.Username}'.");

                return new CommandResult<AdministratorTokenOutputModel>(new OperationError(ValidationErrorCodes.InvalidPassword));
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, Role.User.ToString())
            };

            var token = TokenHelper.Generate(_appSettings.SecretKey, claims, TimeSpan.FromDays(180));

            _logger.Information($"Token was created for user ID = '{parameter.UserId}'.");

            return new CommandResult<AdministratorTokenOutputModel>(new AdministratorTokenOutputModel(token));
        }

        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null)
                return false;

            if (string.IsNullOrWhiteSpace(password))
            {
                _logger.Error("Value cannot be empty or whitespace only string.");

                return false;
            }

            if (storedHash.Length != 64)
            {
                _logger.Error("Invalid length of password hash (64 bytes expected).");

                return false;
            }

            if (storedSalt.Length != 128)
            {
                _logger.Error("Invalid length of password salt (128 bytes expected).");

                return false;
            }

            using (var hmac = new HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (var i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i])
                        return false;
                }
            }

            return true;
        }
    }
}
