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
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TrainingDbContext).Assembly);
        }
    }
}
