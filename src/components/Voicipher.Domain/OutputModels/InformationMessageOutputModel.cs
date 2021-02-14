using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Voicipher.Domain.OutputModels
{
    public record InformationMessageOutputModel
    {
        public InformationMessageOutputModel()
        {
            LanguageVersions = new List<LanguageVersionOutputModel>();
        }

        [Required]
        public Guid Id { get; init; }

        [Required]
        public bool IsUserSpecific { get; init; }

        [Required]
        public bool WasOpened { get; init; }

        [Required]
        public DateTime? DateUpdatedUtc { get; init; }

        [Required]
        public DateTime DatePublishedUtc { get; init; }

        public IList<LanguageVersionOutputModel> LanguageVersions { get; }
    }
}
