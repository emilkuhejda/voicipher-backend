using System;

namespace Voicipher.Domain.Exceptions
{
    public class NotificationAlreadySentException : Exception
    {
        public NotificationAlreadySentException()
        {
        }

        public NotificationAlreadySentException(string message)
            : base(message)
        {
        }

        public NotificationAlreadySentException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
