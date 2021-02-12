using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Voicipher.Business.Extensions;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.InputModels.MetaData;
using Voicipher.Domain.Interfaces.Queries.ControlPanel;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;
using Voicipher.Domain.Validation;

namespace Voicipher.Business.Queries.ControlPanel
{
    public class GetAdministratorQuery : Query<CreateTokenInputModel, QueryResult<Administrator>>, IGetAdministratorQuery
    {
        private readonly IAdministratorRepository _administratorRepository;
        private readonly ILogger _logger;

        public GetAdministratorQuery(
            IAdministratorRepository administratorRepository,
            ILogger logger)
        {
            _administratorRepository = administratorRepository;
            _logger = logger.ForContext<GetAdministratorQuery>();
        }

        protected override async Task<QueryResult<Administrator>> Execute(CreateTokenInputModel parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var validationResult = parameter.Validate();
            if (!validationResult.IsValid)
            {
                _logger.Error($"User input is invalid. {validationResult.ToJson()}");

                return new QueryResult<Administrator>(validationResult.Errors);
            }

            var administrator = await _administratorRepository.GetByNameAsync(parameter.Username, cancellationToken);
            if (administrator == null)
            {
                _logger.Error($"Administrator '{parameter.Username}' was not found.");

                return new QueryResult<Administrator>(new OperationError(ValidationErrorCodes.AdministratorNotFound));
            }

            validationResult = administrator.Validate();
            if (!validationResult.IsValid)
            {
                _logger.Error($"Administrator '{parameter.Username}' has invalid stored properties.");

                return new QueryResult<Administrator>(new OperationError(ValidationErrorCodes.InvalidStoredValues), validationResult.Errors);
            }

            return new QueryResult<Administrator>(administrator);
        }
    }
}
