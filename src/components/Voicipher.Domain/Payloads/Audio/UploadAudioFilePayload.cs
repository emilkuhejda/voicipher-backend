using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Voicipher.Domain.Interfaces.Validation;
using Voicipher.Domain.Validation;

namespace Voicipher.Domain.Payloads.Audio
{
    public record UploadAudioFilePayload : IValidatable
    {
        public string Name { get; init; }

        public string Language { get; init; }

        public string FileName { get; init; }

        public bool IsPhoneCall { get; init; }

        public DateTime DateCreated { get; init; }

        public Guid ApplicationId { get; init; }

        public IFormFile File { get; init; }

        public ValidationResult Validate()
        {
            IList<ValidationError> errors = new List<ValidationError>();

            errors.ValidateRequired(Name, nameof(Name));
            errors.ValidateRequired(Language, nameof(Language));
            errors.ValidateLanguage(Language, nameof(Language));
            errors.ValidateRequired(FileName, nameof(FileName));
            errors.ValidateDateTime(DateCreated, nameof(DateCreated));
            errors.ValidateGuid(ApplicationId, nameof(ApplicationId));
            errors.ValidateNotNull(File, nameof(File));

            return new ValidationResult(errors);
        }
    }
}
