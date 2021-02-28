using System;
using Voicipher.Domain.Notifications;

namespace Voicipher.Domain.Exceptions
{
    public class NotificationErrorException : Exception
    {
        public NotificationErrorException()
        {
        }

        public NotificationErrorException(NotificationError notificationError)
        {
            NotificationError = notificationError;
        }

        public NotificationErrorException(string message)
            : base(message)
        {
        }

        public NotificationErrorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public NotificationError NotificationError { get; }
    }
}
