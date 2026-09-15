using Training.Application.Contracts;
using Training.Domain.Enums;
using Training.Domain.Interfaces.Repositories;
using Training.Domain.Interfaces.Services;
using Training.Domain.Models;

namespace Training.Application.Services
{
    public class ExerciseService : IExerciseService
    {
        private readonly IExerciseRepository _exerciseRepository;

        public ExerciseService(IExerciseRepository exerciseRepository)
        {
            _exerciseRepository = exerciseRepository;
        }

        public GetQuestion GetExercise()
        {
            var exercise = _exerciseRepository.GetExercise(1);
            var question = new GetQuestion()
            {
                Id = exercise.Id,
                Question = exercise.Question
            };
            return question;
        }
        public ExerciseResult CheckAnswer(AnswerResponse answer)
        {
            var exercise = _exerciseRepository.GetExercise(answer.Id);
            if (answer.Answer != null && exercise.Answer.ToLower().Trim() == answer.Answer.ToLower().Trim())
            {
                return new ExerciseResult()
                {
                    ExerciseStatus = ExerciseStatus.Success,
                    Question = exercise.Question,
                    Answer = answer.Answer,
                    CorrectAnswer = exercise.Answer,
                };
            }
            return new ExerciseResult()
            {
                ExerciseStatus = ExerciseStatus.Failure,
                Question = exercise.Question,
                Answer = answer.Answer,
                CorrectAnswer = exercise.Answer,
            };
        }
    }
}
