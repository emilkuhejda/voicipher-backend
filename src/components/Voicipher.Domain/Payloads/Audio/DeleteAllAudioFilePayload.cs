using System;
using System.Collections.Generic;
using Voicipher.Domain.InputModels.Audio;
using Voicipher.Domain.Interfaces.Validation;
using Voicipher.Domain.Validation;

namespace Voicipher.Domain.Payloads.Audio
{
    public record DeleteAllAudioFilePayload : IValidatable
    {
        public DeleteAllAudioFilePayload(IEnumerable<DeletedAudioFileInputModel> audioFiles, Guid applicationId)
        {
            AudioFiles = audioFiles;
            ApplicationId = applicationId;
        }

        public IEnumerable<DeletedAudioFileInputModel> AudioFiles { get; }

        public Guid ApplicationId { get; }

        public ValidationResult Validate()
        {
            IList<ValidationError> errors = new List<ValidationError>();

            errors.ValidateNotNull(AudioFiles, nameof(AudioFiles));
            errors.ValidateGuid(ApplicationId, nameof(ApplicationId));

            return new ValidationResult(errors);
        }
    }
}
