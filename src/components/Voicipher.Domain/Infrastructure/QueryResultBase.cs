using System.Collections.Generic;
using System.Linq;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Validation;

namespace Voicipher.Domain.Infrastructure
{
    public abstract record QueryResultBase
    {
        protected QueryResultBase(OperationResult result, OperationError error, IEnumerable<ValidationError> validationErrors)
        {
            Result = result;
            Error = error;
            ValidationErrors = validationErrors?.ToList() ?? new List<ValidationError>();
        }

        public OperationResult Result { get; }

        public OperationError Error { get; }

        public IReadOnlyList<ValidationError> ValidationErrors { get; }
    }
}
