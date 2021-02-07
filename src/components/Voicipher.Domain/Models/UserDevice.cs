using System;
using System.Collections.Generic;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Interfaces.Validation;
using Voicipher.Domain.Validation;

namespace Voicipher.Domain.Models
{
    public class UserDevice : EntityBase, IValidatable
    {
        public Guid UserId { get; set; }

        public Guid InstallationId { get; set; }

        public RuntimePlatform RuntimePlatform { get; set; }

        public string InstalledVersionNumber { get; set; }

        public Language Language { get; set; }

        public DateTime DateRegisteredUtc { get; set; }

        public ValidationResult Validate()
        {
            IList<ValidationError> errors = new List<ValidationError>();

            errors.ValidateGuid(Id, nameof(Id), nameof(UserDevice));
            errors.ValidateGuid(UserId, nameof(UserId), nameof(UserDevice));
            errors.ValidateGuid(InstallationId, nameof(InstallationId), nameof(UserDevice));
            errors.ValidateRequired(InstalledVersionNumber, nameof(InstalledVersionNumber), nameof(UserDevice));
            errors.ValidateMaxLength(InstalledVersionNumber, nameof(InstalledVersionNumber), 20, nameof(UserDevice));
            errors.ValidateDateTime(DateRegisteredUtc, nameof(DateRegisteredUtc), nameof(DateRegisteredUtc));

            return new ValidationResult(errors);
        }
    }
}
