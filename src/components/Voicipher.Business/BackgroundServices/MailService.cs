using System;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using Voicipher.Domain.Interfaces.Channels;
using Voicipher.Domain.Models;
using Voicipher.Domain.Settings;

namespace Voicipher.Business.BackgroundServices
{
    public class MailService : BackgroundService
    {
        private readonly IMailProcessingChannel _mailProcessingChannel;
        private readonly AppSettings _appSettings;
        private readonly ILogger _logger;

        public MailService(
            IMailProcessingChannel mailProcessingChannel,
            IOptions<AppSettings> options,
            ILogger logger)
        {
            _mailProcessingChannel = mailProcessingChannel;
            _appSettings = options.Value;
            _logger = logger.ForContext<MailService>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var mailData in _mailProcessingChannel.ReadAllAsync(stoppingToken))
            {
                await SendAsync(mailData, stoppingToken);
            }
        }

        private async Task SendAsync(MailData mailData, CancellationToken cancellationToken)
        {
            try
            {
                _logger.Information("Start sending email");

                var mailConfiguration = _appSettings.MailConfiguration;
                using (var client = new SmtpClient(mailConfiguration.SmtpServer, mailConfiguration.Port))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(mailConfiguration.Username, mailConfiguration.Password);

                    using (var mailMessage = new MailMessage())
                    {
                        mailMessage.From = new MailAddress(mailConfiguration.From, mailConfiguration.DisplayName);
                        mailMessage.To.Add(mailData.Recipient);
                        mailMessage.Body = mailData.Body;
                        mailMessage.Subject = mailData.Subject;
                        await client.SendMailAsync(mailMessage, cancellationToken);

                        _logger.Information("Email was successfully sent");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception occurred during sending email");
            }
        }
    }
}
