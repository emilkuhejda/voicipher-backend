﻿using System;
using System.Collections.Generic;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Interfaces.Validation;
using Voicipher.Domain.Validation;

namespace Voicipher.Domain.Models
{
    public class AudioFile : EntityBase, IValidatable
    {
        public Guid UserId { get; set; }

        public Guid ApplicationId { get; set; }

        public string Name { get; set; }

        public string FileName { get; set; }

        public string Language { get; set; }

        public RecognitionState RecognitionState { get; set; }

        public string OriginalSourceFileName { get; set; }

        public string SourceFileName { get; set; }

        public StorageSetting Storage { get; set; }

        public UploadStatus UploadStatus { get; set; }

        public TimeSpan TotalTime { get; set; }

        public TimeSpan TranscribedTime { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime? DateProcessedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsPermanentlyDeleted { get; set; }

        public bool WasCleaned { get; set; }

        //public virtual IList<TranscribeItemEntity> TranscribeItems { get; set; }

        //public virtual IList<WavPartialFileEntity> WavPartialFiles { get; set; }

        public ValidationResult Validate()
        {
            IList<ValidationError> errors = new List<ValidationError>();

            errors.ValidateGuid(Id, nameof(Id), nameof(AudioFile));
            errors.ValidateGuid(ApplicationId, nameof(ApplicationId), nameof(AudioFile));
            errors.ValidateRequired(Name, nameof(Name), nameof(AudioFile));
            errors.ValidateMaxLength(Name, nameof(Name), 150, nameof(AudioFile));
            errors.ValidateRequired(FileName, nameof(FileName), nameof(AudioFile));
            errors.ValidateMaxLength(FileName, nameof(FileName), 150, nameof(AudioFile));
            errors.ValidateRequired(Language, nameof(Language), nameof(AudioFile));
            errors.ValidateMaxLength(Language, nameof(Language), 20, nameof(AudioFile));
            errors.ValidateNullableMaxLength(OriginalSourceFileName, nameof(OriginalSourceFileName), 100, nameof(AudioFile));
            errors.ValidateNullableMaxLength(SourceFileName, nameof(FileName), 100, nameof(AudioFile));
            errors.ValidateDateTime(DateCreated, nameof(DateCreated), nameof(AudioFile));
            errors.ValidateDateTime(DateUpdatedUtc, nameof(DateUpdatedUtc), nameof(AudioFile));

            return new ValidationResult(errors);
        }
    }
}