using Microsoft.EntityFrameworkCore;
using Training.Domain.Models;

namespace Training.Application.Interfaces
{
    public interface ITrainingDbContext
    {
        DbSet<Lesson> Lessons { get; }

        DbSet<Exercise> Exercises { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
