using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Notifications;

namespace Voicipher.Domain.Interfaces.Services
{
    public interface INotificationService
    {
        Task<Dictionary<Language, NotificationResult>> SendAsync(Guid informationMessageId, Guid? userId = null, CancellationToken cancellationToken = default);
    }
}
