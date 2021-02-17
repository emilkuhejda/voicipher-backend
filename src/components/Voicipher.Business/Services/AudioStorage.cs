using Microsoft.AspNetCore.Hosting;

namespace Voicipher.Business.Services
{
    public class AudioStorage : DiskStorage
    {
        private const string Directory = "audio";

        public AudioStorage(IWebHostEnvironment webHostEnvironment)
            : base(webHostEnvironment, Directory)
        {
        }
    }
}
