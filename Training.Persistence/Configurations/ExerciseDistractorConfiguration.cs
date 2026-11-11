using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Training.Domain.Models;

namespace Training.Persistence.Configurations
{
    public class ExerciseDistractorConfiguration : IEntityTypeConfiguration<ExerciseDistractor>
    {
        public void Configure(EntityTypeBuilder<ExerciseDistractor> distractor)
        {
            distractor.ToTable("ExerciseDistractors");
            distractor.HasKey(x => x.Id);
            distractor.Property(x => x.Text).IsRequired().HasMaxLength(500);
            distractor.HasData(
                new ExerciseDistractor { Id = 1, TokenId = 1, Text = "Go" },
                new ExerciseDistractor { Id = 2, TokenId = 1, Text = "See" },
                new ExerciseDistractor { Id = 3, TokenId = 1, Text = "Make" });
        }
    }
}
