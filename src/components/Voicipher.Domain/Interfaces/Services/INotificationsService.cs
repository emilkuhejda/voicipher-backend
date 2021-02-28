using System;
using System.Threading;
using System.Threading.Tasks;
using Voicipher.Domain.Models;
using Voicipher.Domain.Notifications;

namespace Voicipher.Domain.Interfaces.Services
{
    public interface INotificationsService
    {
        Task<NotificationResult> SendAsync(InformationMessage informationMessage, Guid? userId = null, CancellationToken cancellationToken = default);
    }
}
