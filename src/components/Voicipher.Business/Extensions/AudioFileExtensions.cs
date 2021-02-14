using System;
using Voicipher.Domain.Models;

namespace Voicipher.Business.Extensions
{
    public static class AudioFileExtensions
    {
        public static AudioFile CreateDeletedEntity(this AudioFile audioFile, Guid applicationId)
        {
            return new AudioFile
            {
                Id = audioFile.Id,
                UserId = audioFile.UserId,
                ApplicationId = applicationId,
                Name = "--DELETED--",
                FileName = "--DELETED--",
                Language = string.Empty,
                DateCreated = audioFile.DateCreated,
                DateUpdatedUtc = DateTime.UtcNow,
                IsDeleted = true,
                IsPermanentlyDeleted = true,
                WasCleaned = true
            };
        }
    }
}
