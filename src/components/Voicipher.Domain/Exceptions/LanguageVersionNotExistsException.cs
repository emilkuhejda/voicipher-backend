using System;

namespace Voicipher.Domain.Exceptions
{
    public class LanguageVersionNotExistsException : Exception
    {
        public LanguageVersionNotExistsException()
        {
        }

        public LanguageVersionNotExistsException(string message)
            : base(message)
        {
        }

        public LanguageVersionNotExistsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
