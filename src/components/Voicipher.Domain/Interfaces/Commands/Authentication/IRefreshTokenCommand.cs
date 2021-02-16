using System;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Infrastructure;

namespace Voicipher.Domain.Interfaces.Commands.Authentication
{
    public interface IRefreshTokenCommand : ICommand<TimeSpan, CommandResult<string>>
    {
    }
}
