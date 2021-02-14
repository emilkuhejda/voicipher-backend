using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voicipher.Domain.Models;

namespace Voicipher.DataAccess.EntitiesConfiguration
{
    public class ContactFormConfiguration : IEntityTypeConfiguration<ContactForm>
    {
        public void Configure(EntityTypeBuilder<ContactForm> builder)
        {
            builder.ToTable("ContactForm");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasMaxLength(150).IsRequired();
            builder.Property(x => x.Email).HasMaxLength(150).IsRequired();
            builder.Property(x => x.Message).IsRequired();
            builder.Property(x => x.DateCreatedUtc).IsRequired();
        }
    }
}
