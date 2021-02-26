namespace Voicipher.Business.Services
{
    public interface IZipFileService
    {
        void CreateFromDirectory(string sourceDirectoryName, string destinationArchiveFileName);
    }
}
