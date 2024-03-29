﻿using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Voicipher.Business.Extensions;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Commands.EndUser;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.OutputModels;
using Voicipher.Domain.Payloads.EndUser;

namespace Voicipher.Business.Commands.EndUser
{
    public class UpdateLanguageCommand : Command<UpdateLanguagePayload, CommandResult<OkOutputModel>>, IUpdateLanguageCommand
    {
        private readonly IUserDeviceRepository _userDeviceRepository;
        private readonly ILogger _logger;

        public UpdateLanguageCommand(
            IUserDeviceRepository userDeviceRepository,
            ILogger logger)
        {
            _userDeviceRepository = userDeviceRepository;
            _logger = logger.ForContext<UpdateLanguageCommand>();
        }

        protected override async Task<CommandResult<OkOutputModel>> Execute(UpdateLanguagePayload parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var userId = principal.GetNameIdentifier();
            if (!Enum.IsDefined(typeof(Language), parameter.Language))
            {
                _logger.Error($"[{userId}] Language {parameter.Language} is not supported");

                throw new OperationErrorException(ErrorCode.EC200);
            }

            var userDevice = await _userDeviceRepository.GetByInstallationIdAsync(userId, parameter.InstallationId, cancellationToken);
            if (userDevice != null)
            {
                var lang = (Language)parameter.Language;
                userDevice.Language = lang;

                await _userDeviceRepository.SaveAsync(cancellationToken);

                _logger.Information($"[{userId}] Language {lang} was updated for installation ID = {userDevice.InstallationId}");
            }
            else
            {
                _logger.Verbose($"[{userId}] Try to find last installation");

                userDevice = await _userDeviceRepository.GetLastInstallationAsync(userId, cancellationToken);
                if (userDevice != null)
                {
                    var lastInstallationId = userDevice.InstallationId;
                    var lang = (Language)parameter.Language;
                    userDevice.InstallationId = parameter.InstallationId;
                    userDevice.Language = lang;

                    await _userDeviceRepository.SaveAsync(cancellationToken);

                    _logger.Information($"[{userId}] Language {lang} was updated and installation ID was changed from {lastInstallationId} to {userDevice.InstallationId}");
                }
                else
                {
                    _logger.Information($"[{userId}] Device installation not found");
                }
            }

            return new CommandResult<OkOutputModel>(new OkOutputModel());
        }
    }
}
