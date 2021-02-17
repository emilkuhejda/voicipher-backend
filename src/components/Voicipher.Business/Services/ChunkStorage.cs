using Microsoft.AspNetCore.Hosting;

namespace Voicipher.Business.Services
{
    public class ChunkStorage : DiskStorage
    {
        private const string ChunksDirectory = "chunks";

        public ChunkStorage(IWebHostEnvironment webHostEnvironment)
            : base(webHostEnvironment, ChunksDirectory)
        {
        }
    }
}
