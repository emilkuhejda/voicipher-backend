using System;
using System.Collections.Generic;
using Voicipher.Domain.Interfaces.Validation;
using Voicipher.Domain.Validation;

namespace Voicipher.Domain.Payloads.Audio
{
    public record DeleteFileChunkPayload : IValidatable
    {
        public DeleteFileChunkPayload(Guid fileItemId, Guid applicationId)
        {
            FileItemId = fileItemId;
            ApplicationId = applicationId;
        }

        public Guid FileItemId { get; }

        public Guid ApplicationId { get; }

        public ValidationResult Validate()
        {
            IList<ValidationError> errors = new List<ValidationError>();

            errors.ValidateGuid(FileItemId, nameof(FileItemId));
            errors.ValidateGuid(ApplicationId, nameof(ApplicationId));

            return new ValidationResult(errors);
        }
    }
}
