using Voicipher.Domain.Enums;

namespace Voicipher.Domain.Validation
{
    public record OperationError
    {
        public OperationError(ErrorCode errorCode)
        {
            ErrorCode = errorCode;
        }

        public ErrorCode ErrorCode { get; }
    }
}
