using System;
using System.ComponentModel.DataAnnotations;

namespace Voicipher.Domain.OutputModels
{
    public record LanguageVersionOutputModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid InformationMessageId { get; init; }

        [Required]
        public string Title { get; init; }

        [Required]
        public string Message { get; init; }

        [Required]
        public string Description { get; init; }

        [Required]
        public string LanguageString { get; init; }

        [Required]
        public bool SentOnOsx { get; init; }

        [Required]
        public bool SentOnAndroid { get; init; }
    }
}
