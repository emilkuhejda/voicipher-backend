using System;

namespace Voicipher.Domain.Payloads.EndUser
{
    public record UpdateLanguagePayload
    {
        public UpdateLanguagePayload(Guid installationId, int language)
        {
            InstallationId = installationId;
            Language = language;
        }

        public Guid InstallationId { get; }

        public int Language { get; }
    }
}
