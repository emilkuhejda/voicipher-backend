﻿using System;
using System.Collections.Generic;
using Voicipher.Domain.Interfaces.Validation;
using Voicipher.Domain.Validation;

namespace Voicipher.Domain.Payloads.Audio
{
    public record RestoreAllPayload : IValidatable
    {
        public RestoreAllPayload(IEnumerable<Guid> audioFilesIds, Guid applicationId)
        {
            AudioFilesIds = audioFilesIds;
            ApplicationId = applicationId;
        }

        public IEnumerable<Guid> AudioFilesIds { get; }

        public Guid ApplicationId { get; }

        public ValidationResult Validate()
        {
            IList<ValidationError> errors = new List<ValidationError>();

            errors.ValidateNotNull(AudioFilesIds, nameof(AudioFilesIds));
            errors.ValidateGuid(ApplicationId, nameof(ApplicationId));

            return new ValidationResult(errors);
        }
    }
}
