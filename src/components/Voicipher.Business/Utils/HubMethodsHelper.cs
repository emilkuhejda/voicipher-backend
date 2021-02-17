using System;

namespace Voicipher.Business.Utils
{
    public static class HubMethodsHelper
    {
        private const string FilesListChangedMethod = "file-list";
        private const string RecognitionStateChangedMethod = "recognition-state";

        public static string GetFilesListChangedMethod(Guid userId)
        {
            return $"{FilesListChangedMethod}-{userId}";
        }

        public static string GetRecognitionStateChangedMethod(Guid userId)
        {
            return $"{RecognitionStateChangedMethod}-{userId}";
        }
    }
}
