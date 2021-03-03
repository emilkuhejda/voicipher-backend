using System.Collections.Generic;
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
    }
}
