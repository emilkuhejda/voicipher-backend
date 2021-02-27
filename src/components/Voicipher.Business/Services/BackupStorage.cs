using Microsoft.AspNetCore.Hosting;

namespace Voicipher.Business.Services
{
    public class BackupStorage : DiskStorage
    {
        private const string Directory = "backup";

        public BackupStorage(IWebHostEnvironment webHostEnvironment)
            : base(webHostEnvironment, Directory)
        {
        }
    }
}
