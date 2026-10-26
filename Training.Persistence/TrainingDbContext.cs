using Microsoft.EntityFrameworkCore;
using Training.Application.Interfaces;
using Training.Domain.Models;

namespace Training.Persistence
{
    public class TrainingDbContext : DbContext, ITrainingDbContext
    {
        public TrainingDbContext(DbContextOptions<TrainingDbContext> options)
            : base(options)
        {
        }

        public DbSet<Lesson> Lessons => Set<Lesson>();

        public DbSet<Exercise> Exercises => Set<Exercise>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Lesson>(lesson =>
            {
                lesson.ToTable("Lessons");
                lesson.HasKey(x => x.Id);
                lesson.Property(x => x.Name).IsRequired().HasMaxLength(200);
            lesson.HasMany(x => x.Exercises)
                .WithOne()
                .HasForeignKey("LessonId")
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Exercise>(exercise =>
            {
                exercise.ToTable("Exercises");
                exercise.HasKey(x => x.Id);
                exercise.Property(x => x.Question).IsRequired();
                exercise.Property(x => x.Answer).IsRequired();
            });
        }
    }
}
