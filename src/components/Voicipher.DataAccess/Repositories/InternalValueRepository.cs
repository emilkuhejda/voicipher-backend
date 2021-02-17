using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;

namespace Voicipher.DataAccess.Repositories
{
    public class InternalValueRepository : Repository<InternalValue>, IInternalValueRepository
    {
        public InternalValueRepository(DatabaseContext context)
            : base(context)
        {
        }

        public Task<InternalValue> GetValueAsync(string key)
        {
            return Context.InternalValues.FirstOrDefaultAsync(x => x.Key == key);
        }
    }
}
