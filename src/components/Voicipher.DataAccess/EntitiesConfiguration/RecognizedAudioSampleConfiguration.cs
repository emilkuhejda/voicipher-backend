using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voicipher.Domain.Models;

namespace Voicipher.DataAccess.EntitiesConfiguration
{
    public class RecognizedAudioSampleConfiguration : IEntityTypeConfiguration<RecognizedAudioSample>
    {
        public void Configure(EntityTypeBuilder<RecognizedAudioSample> builder)
        {
            builder.ToTable("RecognizedAudioSample");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.DateCreatedUtc).IsRequired();
        }
    }
}
