using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;

namespace Voicipher.DataAccess.Repositories
{
    public abstract class Repository<T> : IRepository<T>, IDisposable where T : EntityBase
    {
        protected DatabaseContext Context { get; }

        protected Repository(DatabaseContext context)
        {
            Context = context;
        }

        public async Task AddAsync(T entity)
        {
            if (entity == null)
                return;

            await Context.Set<T>().AddAsync(entity);
        }

        public T Update(T entity)
        {
            if (entity == null)
                return default;

            Context.Set<T>().Update(entity);
            return entity;
        }

        public void Remove(T entity)
        {
            if (entity == null)
                return;

            Context.Set<T>().Remove(entity);
        }

        public Task<T> GetAsync(Guid entityId, CancellationToken cancellationToken)
        {
            return Context.Set<T>().SingleOrDefaultAsync(x => x.Id == entityId, cancellationToken);
        }

        public Task<T[]> GetAllAsync(CancellationToken cancellationToken)
        {
            return Context.Set<T>().ToArrayAsync(cancellationToken);
        }

        public async Task SaveAsync(CancellationToken cancellationToken)
        {
            await Context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Context?.Dispose();
            }
        }
    }
}
