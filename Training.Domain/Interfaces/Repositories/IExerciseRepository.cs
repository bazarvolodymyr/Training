using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Training.Domain.Models;

namespace Training.Domain.Interfaces.Repositories
{
    public interface IExerciseRepository
    {
        Exercise GetExercise(int id);
    }
}
