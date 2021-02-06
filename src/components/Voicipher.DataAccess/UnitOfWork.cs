using System;
using System.Threading;
using System.Threading.Tasks;
using Voicipher.Domain.Interfaces.Services;

namespace Voicipher.DataAccess
{
    public sealed class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly DatabaseContext _context;

        public UnitOfWork(DatabaseContext context)
        {
            _context = context;
        }

        public Task SaveAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context?.Dispose();
            }
        }

        // Disposable types implement a finalizer.
        ~UnitOfWork()
        {
            Dispose(false);
        }
    }
}
