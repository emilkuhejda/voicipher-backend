using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Voicipher.Domain.Interfaces.Validation;
using Voicipher.Domain.Validation;
using ValidationResult = Voicipher.Domain.Validation.ValidationResult;

namespace Voicipher.Domain.InputModels
{
    public record UpdateUserTranscriptInputModel : IValidatable
    {
        [Required]
        public Guid TranscribeItemId { get; init; }

        [Required]
        public string Transcript { get; init; }

        [Required]
        public Guid ApplicationId { get; init; }

        public ValidationResult Validate()
        {
            IList<ValidationError> errors = new List<ValidationError>();

            errors.ValidateGuid(TranscribeItemId, nameof(TranscribeItemId));
            errors.ValidateGuid(ApplicationId, nameof(ApplicationId));

            return new ValidationResult(errors);
        }
    }
}
