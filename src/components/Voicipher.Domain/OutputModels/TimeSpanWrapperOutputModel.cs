using System.ComponentModel.DataAnnotations;

namespace Voicipher.Domain.OutputModels
{
    public class TimeSpanWrapperOutputModel
    {
        [Required]
        public long Ticks { get; set; }
    }
}
