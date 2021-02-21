using System.Collections.Generic;
using Voicipher.Domain.Models;

namespace Voicipher.Domain.Transcription
{
    public record RecognizedResult
    {
        public RecognizedResult(bool isIncomplete, IEnumerable<RecognitionAlternative> alternatives)
        {
            IsIncomplete = isIncomplete;
            Alternatives = alternatives;
        }

        public bool IsIncomplete { get; }

        public IEnumerable<RecognitionAlternative> Alternatives { get; }
    }
}
