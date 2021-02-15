using System;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Infrastructure;
using Voicipher.Domain.OutputModels;

namespace Voicipher.Domain.Interfaces.Commands
{
    public interface ICreateSpeechConfigurationCommand : ICommand<Guid, CommandResult<SpeechConfigurationOutputModel>>
    {
    }
}
