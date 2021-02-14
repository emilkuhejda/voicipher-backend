using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Voicipher.Domain.Interfaces.Validation;
using Voicipher.Domain.Validation;
using ValidationResult = Voicipher.Domain.Validation.ValidationResult;

namespace Voicipher.Domain.Payloads.Audio
{
    public record CreateAudioFilePayload : IValidatable
    {
        [Required]
        public string Name { get; init; }

        [Required]
        public string Language { get; init; }

        [Required]
        public string FileName { get; init; }

        [Required]
        public DateTime DateCreated { get; init; }

        [Required]
        public Guid ApplicationId { get; init; }

        public ValidationResult Validate()
        {
            IList<ValidationError> errors = new List<ValidationError>();

            errors.ValidateRequired(Name, nameof(Name));
            errors.ValidateRequired(Language, nameof(Language));
            errors.ValidateLanguage(Language, nameof(Language));
            errors.ValidateRequired(FileName, nameof(FileName));
            errors.ValidateDateTime(DateCreated, nameof(DateCreated));
            errors.ValidateGuid(ApplicationId, nameof(ApplicationId));

            return new ValidationResult(errors);
        }
    }
}
