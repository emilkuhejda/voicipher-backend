using System.Collections.Generic;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Validation;

namespace Voicipher.Domain.Infrastructure
{
    public abstract record CommandResultBase
    {
        public bool IsSuccess => Result == OperationResult.Success;

        public OperationResult Result { get; protected init; }

        public OperationError Error { get; protected init; }

        public IReadOnlyList<ValidationError> ValidationErrors { get; protected init; }
    }
}
