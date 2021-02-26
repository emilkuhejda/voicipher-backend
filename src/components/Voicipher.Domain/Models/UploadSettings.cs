namespace Voicipher.Domain.Models
{
    public record UploadSettings
    {
        public UploadSettings()
            : this(string.Empty, string.Empty)
        {
        }

        public UploadSettings(string folderName, string fileName)
        {
            FolderName = folderName;
            FileName = fileName;
        }

        public string FolderName { get; }

        public string FileName { get; }
    }
}
