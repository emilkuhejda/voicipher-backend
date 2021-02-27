using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Commands.ControlPanel;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;
using Voicipher.Domain.Payloads.ControlPanel;

namespace Voicipher.Business.Commands.ControlPanel
{
    public class UpdateInternalValueCommand<T> : Command<InternalValuePayload<T>, CommandResult>, IUpdateInternalValueCommand<T>
    {
        private readonly IInternalValueRepository _internalValueRepository;

        public UpdateInternalValueCommand(IInternalValueRepository internalValueRepository)
        {
            _internalValueRepository = internalValueRepository;
        }

        protected override async Task<CommandResult> Execute(InternalValuePayload<T> parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var key = parameter.Key.ToString();
            var value = Convert.ToString(parameter.DefaultValue);

            var internalValue = await _internalValueRepository.GetValueAsync(key, cancellationToken);
            if (internalValue == null)
            {
                internalValue = new InternalValue { Id = Guid.NewGuid(), Key = key, Value = value };
                await _internalValueRepository.AddAsync(internalValue);
            }
            else
            {
                internalValue.Value = value;
            }

            await _internalValueRepository.SaveAsync(cancellationToken);

            return new CommandResult();
        }
    }
}
