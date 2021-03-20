using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Voicipher.Domain.Interfaces.Validation;
using Voicipher.Domain.Validation;
using ValidationResult = Voicipher.Domain.Validation.ValidationResult;

namespace Voicipher.Domain.InputModels.Audio
{
    public record UpdateAudioFileInputModel : IValidatable
    {
        [Required]
        [BindProperty(Name = "FileItemId")]
        public Guid AudioFileId { get; init; }

        [Required]
        public string Name { get; init; }

        [Required]
        public string Language { get; init; }

        [Required]
        public bool IsPhoneCall { get; init; }

        [Required]
        public uint StartTimeSeconds { get; init; }

        public TimeSpan StartTime => TimeSpan.FromSeconds(StartTimeSeconds);

        [Required]
        public uint EndTimeSeconds { get; init; }

        public TimeSpan EndTime => TimeSpan.FromSeconds(EndTimeSeconds);

        [Required]
        public Guid ApplicationId { get; init; }

        public ValidationResult Validate()
        {
            IList<ValidationError> errors = new List<ValidationError>();

            errors.ValidateGuid(AudioFileId, nameof(AudioFileId));
            errors.ValidateRequired(Name, nameof(Name));
            errors.ValidateRequired(Language, nameof(Language));
            errors.ValidateLanguage(Language, nameof(Language));
            errors.ValidateLanguageModel(Language, IsPhoneCall, nameof(Language));
            errors.ValidateTimeRange(EndTime, nameof(EndTime), StartTime);
            errors.ValidateGuid(ApplicationId, nameof(ApplicationId));

            return new ValidationResult(errors);
        }
    }
}
