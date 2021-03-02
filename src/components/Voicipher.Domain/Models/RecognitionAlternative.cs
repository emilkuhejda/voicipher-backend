using System.Collections.Generic;

namespace Voicipher.Domain.Models
{
    public class RecognitionAlternative
    {
        public RecognitionAlternative(int resultNumber, string transcript, float confidence, IEnumerable<RecognitionWordInfo> words)
        {
            ResultNumber = resultNumber;
            Transcript = transcript;
            Confidence = confidence;
            Words = words;
        }

        public int ResultNumber { get; }

        public string Transcript { get; }

        public float Confidence { get; }

        public IEnumerable<RecognitionWordInfo> Words { get; }
    }
}
