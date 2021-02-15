using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voicipher.Domain.Models;

namespace Voicipher.DataAccess.EntitiesConfiguration
{
    public class DeletedAccountConfiguration : IEntityTypeConfiguration<DeletedAccount>
    {
        public void Configure(EntityTypeBuilder<DeletedAccount> builder)
        {
            builder.ToTable("DeletedAccount");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.DateDeletedUtc).IsRequired();
        }
    }
}
