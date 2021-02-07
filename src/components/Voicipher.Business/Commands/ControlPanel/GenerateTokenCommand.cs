using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Serilog;
using Voicipher.Business.Infrastructure;
using Voicipher.Business.Utils;
using Voicipher.Common.Helpers;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Commands.ControlPanel;
using Voicipher.Domain.OutputModels.MetaData;
using Voicipher.Domain.Payloads;
using Voicipher.Domain.Settings;
using Voicipher.Domain.Validation;

namespace Voicipher.Business.Commands.ControlPanel
{
    public class GenerateTokenCommand : Command<GenerateTokenPayload, CommandResult<AdministratorTokenOutputModel>>, IGenerateTokenCommand
    {
        private readonly AppSettings _appSettings;
        private readonly ILogger _logger;

        public GenerateTokenCommand(
            IOptions<AppSettings> options,
            ILogger logger)
        {
            _appSettings = options.Value;
            _logger = logger.ForContext<GenerateTokenCommand>();
        }

        protected override async Task<CommandResult<AdministratorTokenOutputModel>> Execute(GenerateTokenPayload parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            if (!PasswordHelper.VerifyPasswordHash(parameter.Password, parameter.PasswordHash, parameter.PasswordSalt))
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
    }
}
