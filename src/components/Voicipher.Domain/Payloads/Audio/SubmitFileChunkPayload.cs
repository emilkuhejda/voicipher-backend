using System;
using System.Collections.Generic;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Interfaces.Validation;
using Voicipher.Domain.Validation;

namespace Voicipher.Domain.Payloads.Audio
{
    public record SubmitFileChunkPayload : IValidatable
    {
        public SubmitFileChunkPayload()
        {
            ChunksStorageSetting = StorageSetting.Azure;
        }

        public Guid AudioFileId { get; init; }

        public int ChunksCount { get; init; }

        public StorageSetting ChunksStorageSetting { get; }

        public Guid ApplicationId { get; init; }

        public ValidationResult Validate()
        {
            IList<ValidationError> errors = new List<ValidationError>();

            errors.ValidateGuid(AudioFileId, nameof(AudioFileId));
            errors.ValidateGuid(ApplicationId, nameof(ApplicationId));

            return new ValidationResult(errors);
        }
    }
}
