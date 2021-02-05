using Voicipher.Domain.Validation;

namespace Voicipher.Domain.Interfaces.Validation
{
    public interface IValidatable
    {
        ValidationResult Validate();
    }
}
