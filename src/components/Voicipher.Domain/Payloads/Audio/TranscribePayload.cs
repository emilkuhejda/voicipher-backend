using System;
using System.Collections.Generic;
using Voicipher.Domain.Interfaces.Validation;
using Voicipher.Domain.Validation;

namespace Voicipher.Domain.Payloads.Audio
{
    public record TranscribePayload : IValidatable
    {
        public TranscribePayload(Guid audioFileId, string language, Guid applicationId)
        {
            AudioFileId = audioFileId;
            Language = language;
            ApplicationId = applicationId;
        }

        public Guid AudioFileId { get; }

        public string Language { get; }

        public Guid ApplicationId { get; }

        public ValidationResult Validate()
        {
            IList<ValidationError> errors = new List<ValidationError>();

            errors.ValidateGuid(AudioFileId, nameof(AudioFileId));
            errors.ValidateRequired(Language, nameof(Language));
            errors.ValidateLanguage(Language, nameof(Language));
            errors.ValidateGuid(ApplicationId, nameof(ApplicationId));

            return new ValidationResult(errors);
        }
    }
}
