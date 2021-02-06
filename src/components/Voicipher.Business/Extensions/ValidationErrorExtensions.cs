using System.Collections.Generic;
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
    }
}
