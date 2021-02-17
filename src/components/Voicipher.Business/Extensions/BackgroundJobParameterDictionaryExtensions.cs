using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Voicipher.Domain.Enums;

namespace Voicipher.Business.Extensions
{
    public static class BackgroundJobParameterDictionaryExtensions
    {
        public static void AddOrUpdate(this Dictionary<BackgroundJobParameter, object> dictionary, BackgroundJobParameter key, object value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
            }
            else
            {
                dictionary.Add(key, value);
            }
        }

        public static T GetValue<T>(this Dictionary<BackgroundJobParameter, object> dictionary, BackgroundJobParameter parameter, T defaultValue = default)
        {
            if (dictionary.ContainsKey(parameter))
            {
                var value = dictionary[parameter];
                if (value.GetType() == typeof(T))
                    return (T)Convert.ChangeType(dictionary[parameter], typeof(T));

                return JsonConvert.DeserializeObject<T>(value.ToString() ?? string.Empty);
            }

            return defaultValue;
        }

        public static void Remove(this Dictionary<BackgroundJobParameter, object> dictionary, BackgroundJobParameter parameter)
        {
            if (dictionary.ContainsKey(parameter))
            {
                dictionary.Remove(parameter);
            }
        }
    }
}
