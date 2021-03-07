namespace Voicipher.Domain.Models
{
    public record DiskStorageSettings
    {
        public DiskStorageSettings()
            : this(string.Empty)
        {
        }

        public DiskStorageSettings(string fileName)
            : this(string.Empty, fileName)
        {
        }

        public DiskStorageSettings(string folderName, string fileName)
        {
            FolderName = folderName;
            FileName = fileName;
        }

        public string FolderName { get; }

        public string FileName { get; }
    }
}
