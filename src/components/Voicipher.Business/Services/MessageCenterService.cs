using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Voicipher.Business.Polling;
using Voicipher.Domain.Interfaces.Services;

namespace Voicipher.Business.Services
{
    public class MessageCenterService : IMessageCenterService
    {
        private readonly IHubContext<MessageHub> _messageHub;

        public MessageCenterService(IHubContext<MessageHub> messageHub)
        {
            _messageHub = messageHub;
        }

        public Task SendAsync(string method)
        {
            return _messageHub.Clients.All.SendAsync(method));
        }

        public Task SendAsync(string method, object arg1)
        {
            return _messageHub.Clients.All.SendAsync(method, arg1);
        }

        public Task SendAsync(string method, object arg1, object arg2)
        {
            return _messageHub.Clients.All.SendAsync(method, arg1, arg2);
        }
    }
}
