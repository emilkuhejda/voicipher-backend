using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voicipher.Domain.Models;

namespace Voicipher.DataAccess.EntitiesConfiguration
{
    public class UserDeviceConfiguration : IEntityTypeConfiguration<UserDevice>
    {
        public void Configure(EntityTypeBuilder<UserDevice> builder)
        {
            builder.ToTable("UserDevice");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.InstallationId).IsRequired();
            builder.Property(x => x.RuntimePlatform).IsRequired();
            builder.Property(x => x.InstalledVersionNumber).IsRequired().HasMaxLength(20);
            builder.Property(x => x.Language).IsRequired();
            builder.Property(x => x.DateRegisteredUtc).IsRequired();
        }
    }
}
