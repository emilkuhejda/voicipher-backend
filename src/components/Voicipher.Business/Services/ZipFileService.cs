using System.IO.Compression;

namespace Voicipher.Business.Services
{
    public class ZipFileService : IZipFileService
    {
        public void CreateFromDirectory(string sourceDirectoryName, string destinationArchiveFileName)
        {
            ZipFile.CreateFromDirectory(sourceDirectoryName, destinationArchiveFileName, CompressionLevel.Optimal, true);
        }
    }
}
