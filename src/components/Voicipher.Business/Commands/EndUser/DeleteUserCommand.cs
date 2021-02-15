using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog;
using Voicipher.Business.Extensions;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Commands.EndUser;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.OutputModels;

namespace Voicipher.Business.Commands.EndUser
{
    public class DeleteUserCommand : Command<string, CommandResult<OkOutputModel>>, IDeleteUserCommand
    {
        private readonly IBlobStorage _blobStorage;
        private readonly IUserRepository _userRepository;
        private readonly ILogger _logger;

        public DeleteUserCommand(
            IBlobStorage blobStorage,
            IUserRepository userRepository,
            ILogger logger)
        {
            _blobStorage = blobStorage;
            _userRepository = userRepository;
            _logger = logger.ForContext<DeleteUserCommand>();
        }

        protected override async Task<CommandResult<OkOutputModel>> Execute(string parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var userId = principal.GetNameIdentifier();
            var user = _userRepository.GetByEmailAsync(userId, parameter, cancellationToken);
            if (user == null)
            {
                _logger.Error($"User with ID '{userId}' and email '{parameter}' not found.");

                throw new OperationErrorException(StatusCodes.Status404NotFound);
            }

            return new CommandResult<OkOutputModel>(new OkOutputModel());
        }
    }
}
