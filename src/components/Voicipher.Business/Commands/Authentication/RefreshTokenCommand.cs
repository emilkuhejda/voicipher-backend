using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Serilog;
using Voicipher.Business.Extensions;
using Voicipher.Business.Infrastructure;
using Voicipher.Business.Utils;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Commands.Authentication;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Settings;

namespace Voicipher.Business.Commands.Authentication
{
    public class RefreshTokenCommand : Command<TimeSpan, CommandResult<string>>, IRefreshTokenCommand
    {
        private readonly IUserRepository _userRepository;
        private readonly AppSettings _appSettings;
        private readonly ILogger _logger;

        public RefreshTokenCommand(
            IUserRepository userRepository,
            IOptions<AppSettings> options,
            ILogger logger)
        {
            _userRepository = userRepository;
            _appSettings = options.Value;
            _logger = logger.ForContext<RefreshTokenCommand>();
        }

        protected override async Task<CommandResult<string>> Execute(TimeSpan parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var userId = principal.GetNameIdentifier();
            var exists = await _userRepository.ExistsAsync(userId, cancellationToken);
            if (!exists)
                throw new OperationErrorException(StatusCodes.Status401Unauthorized);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, Role.User.ToString())
            };

            var token = TokenHelper.Generate(_appSettings.SecretKey, claims, parameter);

            _logger.Information($"Token was created for user ID = {userId}");

            return new CommandResult<string>(token);
        }
    }
}
