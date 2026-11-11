using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Training.Domain.Enums;
using Training.Domain.Models;

namespace Training.Persistence.Configurations
{
    public class ExerciseModeConfiguration : IEntityTypeConfiguration<ExerciseMode>
    {
        public void Configure(EntityTypeBuilder<ExerciseMode> mode)
        {
            mode.ToTable("ExerciseModes");
            mode.HasKey(x => new { x.ExerciseId, x.Difficulty });
            mode.Property(x => x.Difficulty).HasConversion<string>().HasMaxLength(16);
            mode.HasData(
                new ExerciseMode { ExerciseId = 1, Difficulty = Difficulty.Easy },
                new ExerciseMode { ExerciseId = 1, Difficulty = Difficulty.Medium },
                new ExerciseMode { ExerciseId = 1, Difficulty = Difficulty.Hard });
        }
    }
}
