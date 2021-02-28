using System;
using System.Collections.Generic;

namespace Voicipher.Domain.Payloads.Notifications
{
    public record InformationMessagePayload
    {
        public InformationMessagePayload()
        {
            LanguageVersions = new List<LanguageVersionPayload>();
        }

        public Guid Id { get; init; }

        public Guid? UserId { get; init; }

        public string CampaignName { get; init; }

        public bool WasOpened { get; init; }

        public DateTime DateCreatedUtc { get; init; }

        public DateTime? DateUpdatedUtc { get; init; }

        public DateTime? DatePublishedUtc { get; init; }

        public IList<LanguageVersionPayload> LanguageVersions { get; init; }
    }
}
