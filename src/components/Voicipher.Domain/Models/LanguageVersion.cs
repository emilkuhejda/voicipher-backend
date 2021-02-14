using System;
using System.Collections.Generic;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Interfaces.Validation;
using Voicipher.Domain.Validation;

namespace Voicipher.Domain.Models
{
    public class LanguageVersion : EntityBase, IValidatable
    {
        public Guid InformationMessageId { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        public string Description { get; set; }

        public Language Language { get; set; }

        public bool SentOnOsx { get; set; }

        public bool SentOnAndroid { get; set; }

        public ValidationResult Validate()
        {
            IList<ValidationError> errors = new List<ValidationError>();

            errors.ValidateGuid(Id, nameof(Id));
            errors.ValidateGuid(InformationMessageId, nameof(InformationMessageId));
            errors.ValidateRequired(Title, nameof(Title));
            errors.ValidateMaxLength(Title, nameof(Title), 150);
            errors.ValidateRequired(Message, nameof(Message));
            errors.ValidateMaxLength(Message, nameof(Message), 150);
            errors.ValidateRequired(Description, nameof(Description));

            return new ValidationResult(errors);
        }
    }
}
