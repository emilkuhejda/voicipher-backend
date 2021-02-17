using System.Collections.Generic;
using Voicipher.Domain.Interfaces.Validation;
using Voicipher.Domain.Validation;

namespace Voicipher.Domain.Models
{
    public class InternalValue : EntityBase, IValidatable
    {
        public string Key { get; set; }

        public string Value { get; set; }
        public ValidationResult Validate()
        {
            IList<ValidationError> errors = new List<ValidationError>();

            errors.ValidateGuid(Id, nameof(Id));
            errors.ValidateRequired(Key, nameof(Key));
            errors.ValidateMaxLength(Key, nameof(Key), 100);
            errors.ValidateRequired(Value, nameof(Value));
            errors.ValidateMaxLength(Value, nameof(Value), 100);

            return new ValidationResult(errors);
        }
    }
}
