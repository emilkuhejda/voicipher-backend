using System;
using System.Collections.Generic;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Models;

namespace Voicipher.Business.Utils
{
    public static class GenericNotifications
    {
        public static InformationMessage GetTranscriptionSuccess(Guid userId, Guid audioFileId)
        {
            var informationMessageId = Guid.NewGuid();

            return new InformationMessage
            {
                Id = informationMessageId,
                UserId = userId,
                CampaignName = $"File transcription: {audioFileId}",
                DateCreatedUtc = DateTime.UtcNow,
                LanguageVersions = new List<LanguageVersion>
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
