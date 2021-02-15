namespace Voicipher.Domain.Models
{
    public record MailData
    {
        public MailData(string recipient, string subject, string body)
        {
            Recipient = recipient;
            Subject = subject;
            Body = body;
        }

        public string Recipient { get; }

        public string Subject { get; }

        public string Body { get; }
    }
}
