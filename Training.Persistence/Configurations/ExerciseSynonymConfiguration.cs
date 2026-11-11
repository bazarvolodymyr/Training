using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Training.Domain.Models;

namespace Training.Persistence.Configurations
{
    public class ExerciseSynonymConfiguration : IEntityTypeConfiguration<ExerciseSynonym>
    {
        public void Configure(EntityTypeBuilder<ExerciseSynonym> synonym)
        {
            synonym.ToTable("ExerciseSynonyms");
            synonym.HasKey(x => x.Id);
            synonym.Property(x => x.Text).IsRequired().HasMaxLength(500);
            synonym.HasData(new ExerciseSynonym
            {
                Id = 1,
                ExerciseId = 1,
                Text = "To know"
            });
        }
    }
}
