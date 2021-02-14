using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;

namespace Voicipher.DataAccess.Repositories
{
    public class ContactFormRepository : Repository<ContactForm>, IContactFormRepository
    {
        public ContactFormRepository(DatabaseContext context)
            : base(context)
        {
        }
    }
}
