using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Voicipher.Domain.Interfaces.Infrastructure;

namespace Voicipher.Business.Infrastructure
{
    public abstract class Command<T, TResult> : ICommand<T, TResult>
    {
        protected abstract Task<TResult> Execute(T parameter, ClaimsPrincipal principal, CancellationToken cancellationToken);

        public async Task<TResult> ExecuteAsync(T parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            return await Execute(parameter, principal, cancellationToken);
        }
    }

    public abstract class Command<T> : ICommand<T>
    {
        protected abstract Task Execute(T parameter, ClaimsPrincipal principal, CancellationToken cancellationToken);

        public async Task ExecuteAsync(T parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            await Execute(parameter, principal, cancellationToken);
        }
    }
}
