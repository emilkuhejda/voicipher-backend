using System.ComponentModel.DataAnnotations;

namespace Voicipher.Domain.OutputModels
{
    public record TimeSpanWrapperOutputModel
    {
        [Required]
        public long Ticks { get; init; }
    }
}
