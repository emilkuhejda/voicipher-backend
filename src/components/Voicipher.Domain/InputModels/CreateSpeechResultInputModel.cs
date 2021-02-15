using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Voicipher.Domain.Interfaces.Validation;
using Voicipher.Domain.Validation;
using ValidationResult = Voicipher.Domain.Validation.ValidationResult;

namespace Voicipher.Domain.InputModels
{
    public record CreateSpeechResultInputModel : IValidatable
    {
        [Required]
        public Guid SpeechResultId { get; init; }

        [Required]
        public Guid RecognizedAudioSampleId { get; init; }

        public string DisplayText { get; init; }

        public ValidationResult Validate()
        {
            IList<ValidationError> errors = new List<ValidationError>();

            errors.ValidateGuid(SpeechResultId, nameof(SpeechResultId));
            errors.ValidateGuid(RecognizedAudioSampleId, nameof(RecognizedAudioSampleId));

            return new ValidationResult(errors);
        }
    }
}
