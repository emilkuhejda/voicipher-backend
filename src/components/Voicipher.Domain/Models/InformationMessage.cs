using System;
using System.Collections.Generic;
using System.Linq;
using Voicipher.Domain.Interfaces.Validation;
using Voicipher.Domain.Validation;

namespace Voicipher.Domain.Models
{
    public class InformationMessage : EntityBase, IValidatable
    {
        public InformationMessage()
        {
            LanguageVersions = new List<LanguageVersion>();
        }

        public Guid? UserId { get; set; }

        public string CampaignName { get; set; }

        public bool WasOpened { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime? DateUpdatedUtc { get; set; }

        public DateTime? DatePublishedUtc { get; set; }

        public IList<LanguageVersion> LanguageVersions { get; set; }

        public ValidationResult Validate()
        {
            IList<ValidationError> errors = new List<ValidationError>();

            errors.ValidateGuid(Id, nameof(Id));
            errors.ValidateRequired(CampaignName, nameof(CampaignName));
            errors.ValidateMaxLength(CampaignName, nameof(CampaignName), 150);
            errors.ValidateDateTime(DateCreatedUtc, nameof(DateCreatedUtc));

            errors.Merge(LanguageVersions.Select(x => x.Validate()).ToList());

            return new ValidationResult(errors);
        }
    }
}
