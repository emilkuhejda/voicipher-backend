using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Microsoft.AspNetCore.Http;
using Serilog;
using Voicipher.Business.Extensions;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Commands.EndUser;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Models;
using Voicipher.Domain.OutputModels;

namespace Voicipher.Business.Commands.EndUser
{
    public class DeleteUserCommand : Command<string, CommandResult<OkOutputModel>>, IDeleteUserCommand
    {
        private readonly IBlobStorage _blobStorage;
        private readonly IUserRepository _userRepository;
        private readonly IDeletedAccountRepository _deletedAccountRepository;
        private readonly ILogger _logger;

        public DeleteUserCommand(
            IBlobStorage blobStorage,
            IUserRepository userRepository,
            IDeletedAccountRepository deletedAccountRepository,
            ILogger logger)
        {
            _blobStorage = blobStorage;
            _userRepository = userRepository;
            _deletedAccountRepository = deletedAccountRepository;
            _logger = logger.ForContext<DeleteUserCommand>();
        }

        protected override async Task<CommandResult<OkOutputModel>> Execute(string parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var userId = principal.GetNameIdentifier();
            _logger.Information($"Start deleting of the user account. User ID = {userId}, email = {parameter}.");

            var user = await _userRepository.GetByEmailAsync(userId, parameter, cancellationToken);
            if (user == null)
            {
                _logger.Error($"User with ID '{userId}' and email '{parameter}' not found.");

                throw new OperationErrorException(StatusCodes.Status404NotFound);
            }

            try
            {
                var deletedAccount = new DeletedAccount
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    DateDeletedUtc = DateTime.UtcNow
                };

                await _deletedAccountRepository.AddAsync(deletedAccount);
                _userRepository.Remove(user);
                await _userRepository.SaveAsync(cancellationToken);

                var blobSettings = new BlobContainerSettings(userId);
                await _blobStorage.DeleteContainer(blobSettings, cancellationToken);

                _logger.Information($"User account with ID = {userId} was successfully deleted.");

                return new CommandResult<OkOutputModel>(new OkOutputModel());
            }
            catch (RequestFailedException ex)
            {
                _logger.Error(ex, $"Blob storage is unavailable. User ID = {userId}.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error occurred during deleting of the user {userId}.");
            }

            throw new OperationErrorException(StatusCodes.Status400BadRequest);
        }
    }
}
