using System;
using Voicipher.Domain.Enums;
using StatusCode = System.Net.HttpStatusCode;

namespace Voicipher.Domain.Exceptions
{
    public class OperationErrorException : Exception
    {
        public OperationErrorException(string message)
            : base(message)
        {
        }

        public OperationErrorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public OperationErrorException(ErrorCode errorCode)
            : this((int)StatusCode.BadRequest, errorCode)
        {
        }

        public OperationErrorException(int httpStatusCode)
            : this(httpStatusCode, ErrorCode.None)
        {
        }

        public OperationErrorException(int httpStatusCode, ErrorCode errorCode)
        {
            HttpStatusCode = httpStatusCode;
            ErrorCode = errorCode;
        }

        public int HttpStatusCode { get; }

        public ErrorCode ErrorCode { get; }
    }
}
