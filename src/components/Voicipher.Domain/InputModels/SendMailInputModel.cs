using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Voicipher.Domain.Interfaces.Validation;
using Voicipher.Domain.Validation;

namespace Voicipher.Domain.InputModels
{
    public record SendMailInputModel : IValidatable
    {
        public Guid FileItemId { get; init; }

        public string Recipient { get; init; }

        public ValidationResult Validate()
        {
            IList<ValidationError> errors = new List<ValidationError>();

            errors.ValidateGuid(FileItemId, nameof(FileItemId));
            errors.ValidateRequired(Recipient, nameof(Recipient));
            errors.ValidateEmail(Recipient, nameof(Recipient));

            return new ValidationResult(errors);
        }
    }
}
