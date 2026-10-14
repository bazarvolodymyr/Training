using MediatR;
using Training.Domain.Enums;
using Training.Domain.Interfaces.Repositories;

namespace Training.Application.Features.Quiz.Commands.CheckAnswer
{
    public class CheckAnswerCommandHandler : IRequestHandler<CheckAnswerCommand, CheckAnswerResponse>
    {
        private readonly IExerciseRepository _exerciseRepository;

        public CheckAnswerCommandHandler(IExerciseRepository exerciseRepository)
        {
            _exerciseRepository = exerciseRepository;
        }

        public Task<CheckAnswerResponse> Handle(CheckAnswerCommand request, CancellationToken cancellationToken)
        {
            var exercise = _exerciseRepository.GetExercise(request.Id);
            var isCorrect = request.Answer != null
                && exercise.Answer.ToLower().Trim() == request.Answer.ToLower().Trim();

            return Task.FromResult(new CheckAnswerResponse
            {
                ExerciseStatus = isCorrect ? ExerciseStatus.Success : ExerciseStatus.Failure,
                Question = exercise.Question,
                Answer = request.Answer,
                CorrectAnswer = exercise.Answer
            });
        }
    }
}
