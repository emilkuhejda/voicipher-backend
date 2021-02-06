using Voicipher.Domain.Enums;

namespace Voicipher.Domain.Validation
{
    public record ValidationError : OperationError
    {
        public ValidationError(ErrorCode errorCode)
            : base(errorCode)
        {
        }

        public ValidationError(ErrorCode errorCode, string field)
            : base(errorCode)
        {
            Field = field;
        }

        public string Field { get; }
    }
}
