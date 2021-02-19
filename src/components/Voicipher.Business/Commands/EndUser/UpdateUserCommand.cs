using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Serilog;
using Voicipher.Business.Extensions;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.InputModels.EndUser;
using Voicipher.Domain.Interfaces.Commands.EndUser;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.OutputModels;

namespace Voicipher.Business.Commands.EndUser
{
    public class UpdateUserCommand : Command<UpdateUserInputModel, CommandResult<IdentityOutputModel>>, IUpdateUserCommand
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public UpdateUserCommand(
            IUserRepository userRepository,
            IMapper mapper,
            ILogger logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger.ForContext<UpdateUserCommand>();
        }

        protected override async Task<CommandResult<IdentityOutputModel>> Execute(UpdateUserInputModel parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var userId = principal.GetNameIdentifier();
            var user = await _userRepository.GetAsync(userId, cancellationToken);
            if (user == null)
            {
                _logger.Error($"[{userId}] User was not found");

                throw new OperationErrorException(StatusCodes.Status401Unauthorized);
            }

            user.GivenName = parameter.GivenName;
            user.FamilyName = parameter.FamilyName;

            _logger.Information($"[{userId}] User was successfully updated");

            var outputModel = _mapper.Map<IdentityOutputModel>(user);
            return new CommandResult<IdentityOutputModel>(outputModel);
        }
    }
}
