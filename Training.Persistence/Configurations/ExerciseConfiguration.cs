using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Training.Domain.Models;

namespace Training.Persistence.Configurations
{
    public class ExerciseConfiguration : IEntityTypeConfiguration<Exercise>
    {
        public void Configure(EntityTypeBuilder<Exercise> exercise)
        {
            exercise.ToTable("Exercises");
            exercise.HasKey(x => x.Id);
            exercise.Property(x => x.Source).IsRequired();
            exercise.Property(x => x.Answer).IsRequired();
            exercise.HasIndex(x => new { x.LessonId, x.SortOrder });

            exercise.HasMany(x => x.Synonyms)
                .WithOne()
                .HasForeignKey(x => x.ExerciseId)
                .OnDelete(DeleteBehavior.Cascade);
            exercise.HasMany(x => x.Tokens)
                .WithOne()
                .HasForeignKey(x => x.ExerciseId)
                .OnDelete(DeleteBehavior.Cascade);
            exercise.HasMany(x => x.Modes)
                .WithOne()
                .HasForeignKey(x => x.ExerciseId)
                .OnDelete(DeleteBehavior.Cascade);

            exercise.HasData(new
            {
                Id = 1,
                LessonId = 1,
                Source = "Знати",
                Answer = "Know",
                SortOrder = 1
            });
        }
    }
}
