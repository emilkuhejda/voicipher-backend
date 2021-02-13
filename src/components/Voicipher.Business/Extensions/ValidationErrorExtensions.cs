using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Voicipher.Domain.Validation;

namespace Voicipher.Business.Extensions
{
    public static class ValidationErrorExtensions
    {
        public static string ToJson(this IReadOnlyList<ValidationError> validationErrors)
        {
            return JsonConvert.SerializeObject(validationErrors);
        }

        public static bool ContainsError(this IReadOnlyList<ValidationError> validationErrors, string field, string errorCode)
        {
            return validationErrors.Any(x =>
                x.Field.Equals(field, StringComparison.OrdinalIgnoreCase) &&
                x.ErrorCode.Equals(errorCode, StringComparison.OrdinalIgnoreCase));
        }
    }
}
