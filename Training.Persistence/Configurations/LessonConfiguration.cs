using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Training.Domain.Models;

namespace Training.Persistence.Configurations
{
    public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
    {
        public void Configure(EntityTypeBuilder<Lesson> lesson)
        {
            lesson.ToTable("Lessons");
            lesson.HasKey(x => x.Id);
            lesson.Property(x => x.Name).IsRequired().HasMaxLength(200);
            lesson.Property(x => x.ArticleHtml).IsRequired();
            lesson.Property(x => x.SourceLang).IsRequired().HasMaxLength(10);
            lesson.Property(x => x.TargetLang).IsRequired().HasMaxLength(10);
            lesson.HasIndex(x => x.IsPublished);
            lesson.HasIndex(x => x.SortOrder);
            lesson.HasMany(x => x.Exercises)
                .WithOne(x => x.Lesson)
                .HasForeignKey(x => x.LessonId)
                .OnDelete(DeleteBehavior.Cascade);

            lesson.HasData(new
            {
                Id = 1,
                Name = "Базові дієслова",
                ArticleHtml = "<p>Знати — <em>know</em>.</p>",
                IsPublished = true,
                SortOrder = 1,
                SourceLang = "uk",
                TargetLang = "en"
            });
        }
    }
}
