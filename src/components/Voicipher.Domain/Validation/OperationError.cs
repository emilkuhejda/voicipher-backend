using Voicipher.Domain.Enums;

namespace Voicipher.Domain.Validation
{
    public class OperationError
    {
        public OperationError(ErrorCode errorCode)
        {
            ErrorCode = errorCode;
        }

        public ErrorCode ErrorCode { get; }
    }
}
