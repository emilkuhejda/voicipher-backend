using System;
using System.Collections.Generic;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Interfaces.Validation;
using Voicipher.Domain.Validation;

namespace Voicipher.Domain.Models
{
    public class TranscribeItem : EntityBase, IValidatable
    {
        public Guid AudioFileId { get; set; }

        public Guid ApplicationId { get; set; }

        public string Alternatives { get; set; }

        public string UserTranscript { get; set; }

        public string SourceFileName { get; set; }

        public StorageSetting Storage { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public TimeSpan TotalTime { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }

        public AudioFile AudioFile { get; set; }

        public ValidationResult Validate()
        {
            IList<ValidationError> errors = new List<ValidationError>();

            errors.ValidateGuid(Id, nameof(Id));
            errors.ValidateGuid(AudioFileId, nameof(AudioFileId));
            errors.ValidateRequired(Alternatives, nameof(Alternatives));
            errors.ValidateRequired(SourceFileName, nameof(SourceFileName));
            errors.ValidateMaxLength(SourceFileName, nameof(SourceFileName), 255);
            errors.ValidateDateTime(DateCreatedUtc, nameof(DateCreatedUtc));
            errors.ValidateDateTime(DateUpdatedUtc, nameof(DateUpdatedUtc));

            return new ValidationResult(errors);
        }
    }
}
