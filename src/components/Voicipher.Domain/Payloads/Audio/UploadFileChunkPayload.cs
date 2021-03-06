using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Voicipher.Domain.Interfaces.Validation;
using Voicipher.Domain.Validation;
using ValidationResult = Voicipher.Domain.Validation.ValidationResult;

namespace Voicipher.Domain.Payloads.Audio
{
    public class UploadFileChunkPayload : IValidatable
    {
        [Required]
        public Guid AudioFileId { get; init; }

        [Required]
        public int Order { get; init; }

        [Required]
        public Guid ApplicationId { get; init; }

        public IFormFile File { get; init; }

        public ValidationResult Validate()
        {
            IList<ValidationError> errors = new List<ValidationError>();

            errors.ValidateGuid(AudioFileId, nameof(AudioFileId));
            errors.ValidateGuid(ApplicationId, nameof(ApplicationId));
            errors.ValidateNotNull(File, nameof(File));

            return new ValidationResult(errors);
        }
    }
}
