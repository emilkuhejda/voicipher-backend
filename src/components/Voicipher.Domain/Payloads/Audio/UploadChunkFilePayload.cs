using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Interfaces.Validation;
using Voicipher.Domain.Validation;
using ValidationResult = Voicipher.Domain.Validation.ValidationResult;

namespace Voicipher.Domain.Payloads.Audio
{
    public class UploadChunkFilePayload : IValidatable
    {
        [Required]
        public Guid FileItemId { get; set; }

        [Required]
        public int Order { get; set; }

        [Required]
        public StorageSetting StorageSetting { get; set; }

        [Required]
        public Guid ApplicationId { get; set; }

        public IFormFile File { get; set; }

        public ValidationResult Validate()
        {
            IList<ValidationError> errors = new List<ValidationError>();

            errors.ValidateGuid(FileItemId, nameof(FileItemId));
            errors.ValidateGuid(ApplicationId, nameof(ApplicationId));
            errors.ValidateNotNull(File, nameof(File));

            return new ValidationResult(errors);
        }
    }
}
