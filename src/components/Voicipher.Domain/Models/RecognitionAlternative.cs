using System.Collections.Generic;

namespace Voicipher.Domain.Models
{
    public class RecognitionAlternative
    {
        public RecognitionAlternative(string transcript, float confidence, IEnumerable<RecognitionWordInfo> words)
        {
            Transcript = transcript;
            Confidence = confidence;
            Words = words;
        }

        public string Transcript { get; }

        public float Confidence { get; }

        public IEnumerable<RecognitionWordInfo> Words { get; }
    }
}
