using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Training.Domain.Models;

namespace Training.Persistence.Configurations
{
    public class ExerciseTokenConfiguration : IEntityTypeConfiguration<ExerciseToken>
    {
        public void Configure(EntityTypeBuilder<ExerciseToken> token)
        {
            token.ToTable("ExerciseTokens");
            token.HasKey(x => x.Id);
            token.Property(x => x.Text).IsRequired().HasMaxLength(500);
            token.HasIndex(x => new { x.ExerciseId, x.SortOrder });
            token.HasMany(x => x.Distractors)
                .WithOne()
                .HasForeignKey(x => x.TokenId)
                .OnDelete(DeleteBehavior.Cascade);
            token.HasData(new
            {
                Id = 1,
                ExerciseId = 1,
                Text = "Know",
                SortOrder = 1
            });
        }
    }
}
