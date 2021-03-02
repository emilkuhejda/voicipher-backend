using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Voicipher.Domain.Models;

namespace Voicipher.Business.Extensions
{
    public static class TranscribeItemExtensions
    {
        public static IEnumerable<RecognitionAlternative> GetAlternatives(this TranscribeItem transcribeItem)
        {
            return JsonConvert.DeserializeObject<IEnumerable<RecognitionAlternative>>(transcribeItem.Alternatives);
        }

        public static IEnumerable<RecognitionAlternative> RemoveInvalidAlternatives(this IEnumerable<RecognitionAlternative> recognitionAlternatives)
        {
            var alternative = recognitionAlternatives
                .GroupBy(x => x.ResultNumber)
                .ToDictionary(x => x.Key, y =>
                {
                    var alternatives = y.ToList();
                    return new
                    {
                        Confidence = alternatives.Any() ? alternatives.Sum(x => x.Confidence) / alternatives.Count : 0,
                        Alternatives = alternatives
                    };
                })
                .OrderByDescending(x => x.Value.Confidence)
                .FirstOrDefault();

            return alternative.Value?.Alternatives ?? Enumerable.Empty<RecognitionAlternative>();
        }
    }
}
