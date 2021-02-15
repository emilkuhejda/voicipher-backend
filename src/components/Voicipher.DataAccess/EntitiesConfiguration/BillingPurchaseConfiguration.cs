using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voicipher.Domain.Models;

namespace Voicipher.DataAccess.EntitiesConfiguration
{
    public class BillingPurchaseConfiguration : IEntityTypeConfiguration<BillingPurchase>
    {
        public void Configure(EntityTypeBuilder<BillingPurchase> builder)
        {
            builder.ToTable("BillingPurchase");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.PurchaseId).IsRequired();
            builder.Property(x => x.ProductId).IsRequired().HasMaxLength(250);
            builder.Property(x => x.AutoRenewing).IsRequired();
            builder.Property(x => x.PurchaseToken).IsRequired();
            builder.Property(x => x.PurchaseState).IsRequired().HasMaxLength(250);
            builder.Property(x => x.ConsumptionState).IsRequired().HasMaxLength(250);
            builder.Property(x => x.Platform).IsRequired().HasMaxLength(250);
            builder.Property(x => x.TransactionDateUtc).IsRequired();
        }
    }
}
