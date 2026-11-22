using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Training.Application.Interfaces;
using Training.Domain.Models;

namespace Training.Persistence
{
    public class TrainingDbContext : IdentityDbContext<TrainingUser, IdentityRole, string>, ITrainingDbContext
    {
        public TrainingDbContext(DbContextOptions<TrainingDbContext> options)
            : base(options)
        {
        }

        public DbSet<Lesson> Lessons => Set<Lesson>();

        public DbSet<Exercise> Exercises => Set<Exercise>();

        public DbSet<Subscription> Subscriptions => Set<Subscription>();

        public DbSet<MediaAsset> MediaAssets => Set<MediaAsset>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TrainingDbContext).Assembly);
        }
    }
}
