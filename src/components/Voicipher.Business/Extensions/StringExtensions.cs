using Voicipher.Domain.Enums;

namespace Voicipher.Business.Extensions
{
    public static class StringExtensions
    {
        public static RuntimePlatform ToRuntimePlatform(this string runtimePlatform)
        {
            switch (runtimePlatform)
            {
                case "Android":
                    return RuntimePlatform.Android;
                case "iOS":
                    return RuntimePlatform.Osx;
                default:
                    return RuntimePlatform.Undefined;
            }
        }

        public static Language ToLanguage(this string language)
        {
            switch (language)
            {
                case "en":
                    return Language.English;
                case "sk":
                    return Language.Slovak;
                default:
                    return Language.Undefined;
            }
        }
    }
}
