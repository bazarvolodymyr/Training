using MediatR;
using Microsoft.EntityFrameworkCore;
using Training.Application.Interfaces;
using Training.Domain.Enums;

namespace Training.Application.Features.Quiz.Commands.CheckAnswer
{
    public class CheckAnswerCommandHandler : IRequestHandler<CheckAnswerCommand, CheckAnswerResponse?>
    {
        private readonly ITrainingDbContext _db;

        public CheckAnswerCommandHandler(ITrainingDbContext db)
        {
            _db = db;
        }

        public async Task<CheckAnswerResponse?> Handle(CheckAnswerCommand request, CancellationToken cancellationToken)
        {
            var exercise = await _db.Exercises.AsNoTracking()
                .FirstOrDefaultAsync(item => item.Id == request.Id, cancellationToken);
            if (exercise is null)
            {
                return null;
            }

            var isCorrect = request.Answer != null
                && exercise.Answer.ToLower().Trim() == request.Answer.ToLower().Trim();

            return new CheckAnswerResponse
            {
                ExerciseStatus = isCorrect ? ExerciseStatus.Success : ExerciseStatus.Failure,
                Question = exercise.Source,
                Answer = request.Answer,
                CorrectAnswer = exercise.Answer
            };
        }
    }
}
