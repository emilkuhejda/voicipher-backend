using System.ComponentModel.DataAnnotations;

namespace Voicipher.Domain.OutputModels
{
    public record TimeSpanWrapperOutputModel
    {
        public TimeSpanWrapperOutputModel(long ticks)
        {
            Ticks = ticks;
        }

        [Required]
        public long Ticks { get; }
    }
}
