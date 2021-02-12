using System;

namespace Voicipher.Business.Utils
{
    public static class HubMethodsHelper
    {
        private const string FilesListChangedMethod = "file-list";

        public static string GetFilesListChangedMethod(Guid userId)
        {
            return $"{FilesListChangedMethod}-{userId}";
        }
    }
}
