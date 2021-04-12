using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Voicipher.Domain.Models;
using Voicipher.Domain.OutputModels.Audio;

namespace Voicipher.Business.Extensions
{
    public static class TranscribeItemExtensions
    {
        public static IEnumerable<RecognitionAlternative> GetAlternatives(this TranscribeItem transcribeItem)
        {
            return JsonConvert.DeserializeObject<IEnumerable<RecognitionAlternative>>(transcribeItem.Alternatives);
        }

        public static IEnumerable<TranscribeItemOutputModel> RemoveAlternatives(this IEnumerable<TranscribeItemOutputModel> transcribeItems)
        {
            var outputModels = new List<TranscribeItemOutputModel>();
            foreach (var transcribeItem in transcribeItems)
            {
                var alternatives = transcribeItem.Alternatives
                    .GroupBy(x => x.StatTime)
                    .Select(x => x.FirstOrDefault())
                    .ToList();

                outputModels.Add(transcribeItem with { Alternatives = alternatives });
            }

            return outputModels;
        }
    }
}
