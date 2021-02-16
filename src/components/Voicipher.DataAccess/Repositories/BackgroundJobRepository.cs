using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;

namespace Voicipher.DataAccess.Repositories
{
    public class BackgroundJobRepository : Repository<BackgroundJob>, IBackgroundJobRepository
    {
        public BackgroundJobRepository(DatabaseContext context)
            : base(context)
        {
        }
    }
}
