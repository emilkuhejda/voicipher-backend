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
        public Guid SpeechResultId { get; set; }

        [Required]
        public Guid RecognizedAudioSampleId { get; set; }

        public string DisplayText { get; set; }

        public ValidationResult Validate()
        {
            IList<ValidationError> errors = new List<ValidationError>();

            errors.ValidateGuid(SpeechResultId, nameof(SpeechResultId));
            errors.ValidateGuid(RecognizedAudioSampleId, nameof(RecognizedAudioSampleId));

            return new ValidationResult(errors);
        }
    }
}
