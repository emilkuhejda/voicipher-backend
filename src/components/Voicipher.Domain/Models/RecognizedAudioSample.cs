using System;
using System.Collections.Generic;
using System.Linq;
using Voicipher.Domain.Interfaces.Validation;
using Voicipher.Domain.Validation;

namespace Voicipher.Domain.Models
{
    public class RecognizedAudioSample : EntityBase, IValidatable
    {
        public RecognizedAudioSample()
        {
            SpeechResults = new List<SpeechResult>();
        }

        public Guid UserId { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public IList<SpeechResult> SpeechResults { get; set; }

        public ValidationResult Validate()
        {
            IList<ValidationError> errors = new List<ValidationError>();

            errors.ValidateGuid(Id, nameof(Id));
            errors.ValidateDateTime(DateCreatedUtc, nameof(DateCreatedUtc));
            errors.Merge(SpeechResults.Select(x => x.Validate()).ToList());

            return new ValidationResult(errors);
        }
    }
}
