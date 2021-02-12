using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;

namespace Voicipher.DataAccess.Repositories
{
    public class AudioFileRepository : Repository<AudioFile>, IAudioFileRepository
    {
        public AudioFileRepository(DatabaseContext context)
            : base(context)
        {
        }
    }
}
