using System;
using System.Collections.Generic;
using Voicipher.Domain.Interfaces.Validation;
using Voicipher.Domain.Validation;

namespace Voicipher.Domain.Models
{
    public class FileChunk : EntityBase, IValidatable
    {
        public Guid FileItemId { get; set; }

        public Guid ApplicationId { get; set; }

        public int Order { get; set; }

        public string Path { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public ValidationResult Validate()
        {
            IList<ValidationError> errors = new List<ValidationError>();

            errors.ValidateGuid(Id, nameof(Id));
            errors.ValidateGuid(FileItemId, nameof(FileItemId));
            errors.ValidateGuid(ApplicationId, nameof(ApplicationId));
            errors.ValidateRequired(Path, nameof(Path));
            errors.ValidateDateTime(DateCreatedUtc, nameof(DateCreatedUtc));

            return new ValidationResult(errors);
        }
    }
}
