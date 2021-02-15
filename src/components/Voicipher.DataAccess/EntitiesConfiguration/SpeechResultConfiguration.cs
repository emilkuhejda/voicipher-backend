using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voicipher.Domain.Models;

namespace Voicipher.DataAccess.EntitiesConfiguration
{
    public class SpeechResultConfiguration : IEntityTypeConfiguration<SpeechResult>
    {
        public void Configure(EntityTypeBuilder<SpeechResult> builder)
        {
            builder.ToTable("SpeechResult");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.RecognizedAudioSampleId).IsRequired();
        }
    }
}
