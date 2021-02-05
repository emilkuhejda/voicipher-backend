using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Voicipher.Domain.Validation
{
    public readonly struct ValidationResult
    {
        private static readonly IReadOnlyList<ValidationError> EmptyErrorList = new ReadOnlyCollection<ValidationError>(new List<ValidationError>());

        private readonly IReadOnlyList<ValidationError> _errors;

        public ValidationResult(IReadOnlyList<ValidationError> errors)
        {
            _errors = errors;
        }

        public ValidationResult(IList<ValidationError> errors)
        {
            _errors = errors != null ? new ReadOnlyCollection<ValidationError>(errors) : EmptyErrorList;
        }

        public static ValidationResult Success { get; } = default;

        public bool IsValid => _errors == null || _errors.Count == 0;

        public IReadOnlyList<ValidationError> Errors => _errors ?? EmptyErrorList;
    }
}
