using System.Collections.Generic;
using Voicipher.Domain.Validation;

namespace Voicipher.Domain.OutputModels
{
    public record ErrorResultOutputModel
    {
        public OperationError Error { get; init; }

        public IReadOnlyList<ValidationError> ValidationErrors { get; init; }
    }
}
