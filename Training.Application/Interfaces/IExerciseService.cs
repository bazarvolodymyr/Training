using Training.Application.Contracts;
using Training.Domain.Models;

namespace Training.Domain.Interfaces.Services
{
    public interface IExerciseService
    {
        GetQuestion GetExercise();
        ExerciseResult CheckAnswer(AnswerResponse answer);
    }
}
