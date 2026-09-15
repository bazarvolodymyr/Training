using Training.Domain.Interfaces.Repositories;
using Training.Domain.Models;

namespace Training.Persistence.Repositories
{
    public class ExerciseRepository : IExerciseRepository
    {
        public Exercise GetExercise(int id)
        {
           return new Exercise()
           {
               Id = 1,
               Question = "Знати",
               Answer = "Know"
           };
        }
    }
}
