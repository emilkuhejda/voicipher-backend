using System;
using System.Collections.Generic;
using Voicipher.Domain.Interfaces.Validation;
using Voicipher.Domain.Validation;

namespace Voicipher.Domain.Payloads.Audio
{
    public record TranscribePayload : IValidatable
    {
        public TranscribePayload(Guid audioFileId, string language, uint startTimeSeconds, uint endTimeSeconds, Guid applicationId)
        {
            AudioFileId = audioFileId;
            Language = language;
            StartTime = TimeSpan.FromSeconds(startTimeSeconds);
            EndTime = TimeSpan.FromSeconds(endTimeSeconds);
            ApplicationId = applicationId;
        }

        public Guid AudioFileId { get; }

        public string Language { get; }

        public TimeSpan StartTime { get; }

        public TimeSpan EndTime { get; }

        public Guid ApplicationId { get; }

        public ValidationResult Validate()
        {
            IList<ValidationError> errors = new List<ValidationError>();

            errors.ValidateGuid(AudioFileId, nameof(AudioFileId));
            errors.ValidateRequired(Language, nameof(Language));
            errors.ValidateLanguage(Language, nameof(Language));
            errors.ValidateGuid(ApplicationId, nameof(ApplicationId));
            errors.ValidateTimeRange(EndTime, nameof(EndTime), StartTime);

            return new ValidationResult(errors);
        }
    }
}
