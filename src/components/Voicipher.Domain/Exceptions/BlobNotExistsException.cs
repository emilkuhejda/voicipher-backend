using System;

namespace Voicipher.Domain.Exceptions
{
    public class BlobNotExistsException : Exception
    {
        public BlobNotExistsException()
        {
        }

        public BlobNotExistsException(string message)
            : base(message)
        {
        }

        public BlobNotExistsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
