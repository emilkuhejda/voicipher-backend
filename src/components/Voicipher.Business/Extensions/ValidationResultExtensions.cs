using Newtonsoft.Json;
using Voicipher.Domain.Validation;

namespace Voicipher.Business.Extensions
{
    public static class ValidationResultExtensions
    {
        public static string ToJson(this ValidationResult validationResult)
        {
            return JsonConvert.SerializeObject(validationResult);
        }
    }
}
