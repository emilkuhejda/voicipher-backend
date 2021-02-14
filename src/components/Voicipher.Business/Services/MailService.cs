using System.Threading.Tasks;
using Voicipher.Domain.Interfaces.Services;

namespace Voicipher.Business.Services
{
    public class MailService : IMailService
    {
        public Task SendAsync(string recipient, string subject, string body)
        {
        }
    }
}
