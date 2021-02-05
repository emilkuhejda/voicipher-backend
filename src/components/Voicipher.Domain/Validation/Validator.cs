using System.Collections.Generic;

namespace Voicipher.Domain.Validation
{
    public static class Validator
    {
        public static IList<ValidationError> Merge(this IList<ValidationError> errorList, ValidationResult validationResult)
        {
            var errors = errorList ?? new List<ValidationError>();

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    errors.Add(error);
                }
            }

            return errors;
        }
    }
}
