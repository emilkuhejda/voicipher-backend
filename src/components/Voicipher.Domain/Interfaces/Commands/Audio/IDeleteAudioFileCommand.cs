﻿using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Infrastructure;
using Voicipher.Domain.OutputModels;
using Voicipher.Domain.Payloads.Audio;

namespace Voicipher.Domain.Interfaces.Commands.Audio
{
    public interface IDeleteAudioFileCommand : ICommand<DeleteAudioFilePayload, CommandResult<OkOutputModel>>
    {
    }
}
