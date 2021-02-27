using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Queries.ControlPanel;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.OutputModels.ControlPanel;
using Voicipher.Domain.Payloads.ControlPanel;

namespace Voicipher.Business.Queries.ControlPanel
{
    public class GetInternalValueQuery<T> : Query<GetInternalValuePayload<T>, QueryResult<InternalValueOutputModel<T>>>, IGetInternalValueQuery<T>
    {
        private readonly IInternalValueRepository _internalValueRepository;

        public GetInternalValueQuery(IInternalValueRepository internalValueRepository)
        {
            _internalValueRepository = internalValueRepository;
        }

        protected override async Task<QueryResult<InternalValueOutputModel<T>>> Execute(GetInternalValuePayload<T> parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var internalValue = await _internalValueRepository.GetValueAsync(parameter.Key.ToString(), cancellationToken);
            if (internalValue == null)
                return new QueryResult<InternalValueOutputModel<T>>(new InternalValueOutputModel<T>(parameter.DefaultValue));

            var value = ParseResult(internalValue.Value, parameter.DefaultValue);
            return new QueryResult<InternalValueOutputModel<T>>(new InternalValueOutputModel<T>(value));
        }

        private T ParseResult<T>(string value, T defaultValue)
        {
            try
            {
                if (typeof(T).IsEnum)
                {
                    return (T)Enum.Parse(typeof(T), value);
                }

                var result = (T)Convert.ChangeType(value, typeof(T));
                return result;
            }
            catch (NotSupportedException)
            {
                return defaultValue;
            }
        }
    }
}
