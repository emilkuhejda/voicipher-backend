using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Models;
using Voicipher.Domain.Notifications;

namespace Voicipher.Domain.Interfaces.Services
{
    public interface INotificationService
    {
        Task<Dictionary<Language, NotificationResult>> SendAsync(InformationMessage informationMessage, Guid? userId = null, CancellationToken cancellationToken = default);
    }
}
