using System;

namespace Voicipher.Domain.Models
{
    public abstract class EntityBase : IDisposable
    {
        public Guid Id { get; set; }

        public virtual void Dispose()
        {
        }
    }
}
