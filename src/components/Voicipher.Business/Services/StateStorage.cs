using Microsoft.AspNetCore.Hosting;

namespace Voicipher.Business.Services
{
    public class StateStorage : DiskStorage
    {
        private const string Directory = "state";

        public StateStorage(IWebHostEnvironment webHostEnvironment)
            : base(webHostEnvironment, Directory)
        {
        }
    }
}
