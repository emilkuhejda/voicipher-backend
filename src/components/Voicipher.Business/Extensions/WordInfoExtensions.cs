using System.Collections.Generic;
using System.Linq;
using Google.Cloud.Speech.V1;
using Google.Protobuf.Collections;
using Voicipher.Domain.Models;

namespace Voicipher.Business.Extensions
{
    public static class WordInfoExtensions
    {
        public static IEnumerable<RecognitionWordInfo> ToRecognitionWords(this RepeatedField<WordInfo> words)
        {
            return words.Select(x => new RecognitionWordInfo
            {
                Word = x.Word,
                StartTime = x.StartTime.ToTimeSpan(),
                EndTime = x.EndTime.ToTimeSpan(),
                SpeakerTag = x.SpeakerTag
            });
        }
    }
}
