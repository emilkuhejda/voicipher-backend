﻿using System.Collections.Generic;

namespace Voicipher.Domain.Utils
{
    /// <summary>
    /// https://cloud.google.com/speech-to-text/docs/languages
    /// </summary>
    public static class SupportedLanguages
    {
        public static bool IsPhoneCallModelSupported(string language)
        {
            return PhoneCallModels.Contains(language);
        }

        public static bool IsSupported(string language)
        {
            return Languages.Contains(language);
        }

        private static IList<string> PhoneCallModels { get; } = new List<string>
        {
            "en-GB",
            "en-US",
            "ru-RU"
        };

        private static IList<string> Languages { get; } = new List<string>
        {
            "af-ZA",
            "am-ET",
            "hy-AM",
            "az-AZ",
            "id-ID",
            "ms-MY",
            "bn-BD",
            "bn-IN",
            "ca-ES",
            "cs-CZ",
            "da-DK",
            "de-DE",
            "en-AU",
            "en-CA",
            "en-GH",
            "en-GB",
            "en-IN",
            "en-IE",
            "en-KE",
            "en-NZ",
            "en-NG",
            "en-PH",
            "en-SG",
            "en-ZA",
            "en-TZ",
            "en-US",
            "es-AR",
            "es-BO",
            "es-CL",
            "es-CO",
            "es-CR",
            "es-EC",
            "es-SV",
            "es-ES",
            "es-US",
            "es-GT",
            "es-HN",
            "es-MX",
            "es-NI",
            "es-PA",
            "es-PY",
            "es-PE",
            "es-PR",
            "es-DO",
            "es-UY",
            "es-VE",
            "eu-ES",
            "fil-PH",
            "fr-CA",
            "fr-FR",
            "gl-ES",
            "ka-GE",
            "gu-IN",
            "hr-HR",
            "zu-ZA",
            "is-IS",
            "it-IT",
            "jv-ID",
            "kn-IN",
            "km-KH",
            "lo-LA",
            "lv-LV",
            "lt-LT",
            "hu-HU",
            "ml-IN",
            "mr-IN",
            "nl-NL",
            "ne-NP",
            "nb-NO",
            "pl-PL",
            "pt-BR",
            "pt-PT",
            "ro-RO",
            "si-LK",
            "sk-SK",
            "sl-SI",
            "su-ID",
            "sw-TZ",
            "sw-KE",
            "fi-FI",
            "sv-SE",
            "ta-IN",
            "ta-SG",
            "ta-LK",
            "ta-MY",
            "te-IN",
            "vi-VN",
            "tr-TR",
            "ur-PK",
            "ur-IN",
            "el-GR",
            "bg-BG",
            "ru-RU",
            "sr-RS",
            "uk-UA",
            "he-IL",
            "ar-IL",
            "ar-JO",
            "ar-AE",
            "ar-BH",
            "ar-DZ",
            "ar-SA",
            "ar-IQ",
            "ar-KW",
            "ar-MA",
            "ar-TN",
            "ar-OM",
            "ar-PS",
            "ar-QA",
            "ar-LB",
            "ar-EG",
            "fa-IR",
            "hi-IN",
            "th-TH",
            "ko-KR",
            "zh-TW",
            "yue-Hant-HK",
            "ja-JP",
            "zh-HK",
            "zh"
        };
    }
}
