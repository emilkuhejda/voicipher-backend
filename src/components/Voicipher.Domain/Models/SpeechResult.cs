using System;
using System.Collections.Generic;
using Voicipher.Domain.Interfaces.Validation;
using Voicipher.Domain.Validation;

namespace Voicipher.Domain.Models
{
    public class SpeechResult : EntityBase, IValidatable
    {
        public Guid RecognizedAudioSampleId { get; set; }

        public string DisplayText { get; set; }

        public TimeSpan TotalTime { get; set; }

        public ValidationResult Validate()
        {
            IList<ValidationError> errors = new List<ValidationError>();

            errors.ValidateGuid(Id, nameof(Id));
            errors.ValidateGuid(RecognizedAudioSampleId, nameof(RecognizedAudioSampleId));

            return new ValidationResult(errors);
        }
    }
}
