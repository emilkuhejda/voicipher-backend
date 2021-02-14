using System;
using System.Collections.Generic;
using Voicipher.Domain.Interfaces.Validation;
using Voicipher.Domain.Validation;

namespace Voicipher.Domain.Payloads.Audio
{
    public record DeleteAudioFilePayload : IValidatable
    {
        public DeleteAudioFilePayload(Guid audioFileId, Guid applicationId)
        {
            AudioFileId = audioFileId;
            ApplicationId = applicationId;
        }

        public Guid AudioFileId { get; }

        public Guid ApplicationId { get; }

        public ValidationResult Validate()
        {
            IList<ValidationError> errors = new List<ValidationError>();

            errors.ValidateGuid(AudioFileId, nameof(AudioFileId));
            errors.ValidateGuid(ApplicationId, nameof(ApplicationId));

            return new ValidationResult(errors);
        }
    }
}
