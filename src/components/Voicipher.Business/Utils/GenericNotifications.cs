using System;
using System.Collections.Generic;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Payloads.Notifications;

namespace Voicipher.Business.Utils
{
    public static class GenericNotifications
    {
        public static InformationMessagePayload GetTranscriptionSuccess(Guid userId, Guid audioFileId)
        {
            var informationMessageId = Guid.NewGuid();
            var utcNow = DateTime.UtcNow;

            return new InformationMessagePayload
            {
                Id = informationMessageId,
                UserId = userId,
                CampaignName = $"File transcription: {audioFileId}",
                DateCreatedUtc = utcNow,
                DateUpdatedUtc = utcNow,
                DatePublishedUtc = utcNow,
                LanguageVersions = new List<LanguageVersionPayload>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        InformationMessageId = informationMessageId,
                        Title = "Voicipher finished task",
                        Message = "Your file was transcripted",
                        Description = "Thanks God, your file was transcripted.",
                        Language = Language.English
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        InformationMessageId = informationMessageId,
                        Title = "Voicipher finished task",
                        Message = "Your file was transcripted",
                        Description = "Thanks God, your file was transcripted.",
                        Language = Language.Slovak
                    }
                }
            };
        }
    }
}
