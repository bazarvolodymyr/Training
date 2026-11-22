using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Training.Domain.Models;

namespace Training.Persistence.Configurations
{
    public class MediaAssetConfiguration : IEntityTypeConfiguration<MediaAsset>
    {
        public void Configure(EntityTypeBuilder<MediaAsset> media)
        {
            media.ToTable("MediaAssets");
            media.HasKey(x => x.Id);
            media.Property(x => x.Kind).HasConversion<string>().HasMaxLength(16);
            media.Property(x => x.ContentType).IsRequired().HasMaxLength(100);
            media.Property(x => x.OriginalFileName).IsRequired().HasMaxLength(200);
            media.Property(x => x.StoredFileName).IsRequired().HasMaxLength(64);
            media.HasIndex(x => x.LessonId);
            media.HasOne(x => x.Lesson)
                .WithMany()
                .HasForeignKey(x => x.LessonId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
