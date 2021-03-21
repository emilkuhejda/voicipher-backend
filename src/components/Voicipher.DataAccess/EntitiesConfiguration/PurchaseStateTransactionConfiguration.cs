using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voicipher.Domain.Models;

namespace Voicipher.DataAccess.EntitiesConfiguration
{
    public class PurchaseStateTransactionConfiguration : IEntityTypeConfiguration<PurchaseStateTransaction>
    {
        public void Configure(EntityTypeBuilder<PurchaseStateTransaction> builder)
        {
            builder.ToTable("PurchaseStateTransaction");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.BillingPurchaseId).IsRequired();
            builder.Property(x => x.PreviousPurchaseState).IsRequired().HasMaxLength(250);
            builder.Property(x => x.PurchaseState).IsRequired().HasMaxLength(250);
            builder.Property(x => x.TransactionDateUtc).IsRequired();
        }
    }
}
