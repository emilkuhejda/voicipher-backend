using System;
using Voicipher.Domain.Enums;

namespace Voicipher.Domain.Payloads.Notifications
{
    public record LanguageVersionPayload
    {
        public Guid Id { get; init; }

        public Guid InformationMessageId { get; init; }

        public string Title { get; init; }

        public string Message { get; init; }

        public string Description { get; init; }

        public Language Language { get; init; }

        public bool SentOnOsx { get; init; }

        public bool SentOnAndroid { get; init; }
    }
}
