using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Serilog;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Settings;

namespace Voicipher.Business.Services
{
    public class MailService : IMailService
    {
        private readonly AppSettings _appSettings;
        private readonly ILogger _logger;

        public MailService(
            IOptions<AppSettings> options,
            ILogger logger)
        {
            _appSettings = options.Value;
            _logger = logger.ForContext<MailService>();
        }

        public async Task SendAsync(string recipient, string subject, string body)
        {
            try
            {
                var mailConfiguration = _appSettings.MailConfiguration;
                using (var client = new SmtpClient(mailConfiguration.SmtpServer, mailConfiguration.Port))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(mailConfiguration.Username, mailConfiguration.Password);

                    using (MailMessage mailMessage = new MailMessage())
                    {
                        mailMessage.From = new MailAddress(mailConfiguration.From, mailConfiguration.DisplayName);
                        mailMessage.To.Add(recipient);
                        mailMessage.Body = body;
                        mailMessage.Subject = subject;
                        await client.SendMailAsync(mailMessage);

                        _logger.Information($"Email was successfully sent to recipient: '{recipient}'.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception occurred during sending email.");
            }
        }
    }
}
