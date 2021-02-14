using System.Threading.Tasks;

namespace Voicipher.Domain.Interfaces.Services
{
    public interface IMailService
    {
        Task SendAsync(string recipient, string subject, string body);
    }
}
