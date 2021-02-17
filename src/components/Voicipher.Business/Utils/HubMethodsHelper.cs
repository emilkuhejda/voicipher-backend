using System;

namespace Voicipher.Business.Utils
{
    public static class HubMethodsHelper
    {
        private const string RecognitionProgressChangedMethod = "recognition-progress";
        private const string FilesListChangedMethod = "file-list";
        private const string RecognitionStateChangedMethod = "recognition-state";
        private const string RecognitionErrorMethod = "recognition-error";

        public static string GetRecognitionProgressChangedMethod(Guid userId)
        {
            return $"{RecognitionProgressChangedMethod}-{userId}";
        }

        public static string GetFilesListChangedMethod(Guid userId)
        {
            return $"{FilesListChangedMethod}-{userId}";
        }

        public static string GetRecognitionStateChangedMethod(Guid userId)
        {
            return $"{RecognitionStateChangedMethod}-{userId}";
        }

        public static string GetRecognitionErrorMethod(Guid userId)
        {
            return $"{RecognitionErrorMethod}-{userId}";
        }
    }
}
