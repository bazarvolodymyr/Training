using MediatR;
using Microsoft.EntityFrameworkCore;
using Training.Application.Interfaces;
using Training.Domain.Enums;

namespace Training.Application.Features.Quiz.Commands.SubmitAttempt
{
    public class SubmitAttemptCommandHandler : IRequestHandler<SubmitAttemptCommand, SubmitAttemptResponse?>
    {
        private readonly ITrainingDbContext _db;

        public SubmitAttemptCommandHandler(ITrainingDbContext db)
        {
            _db = db;
        }

        public async Task<SubmitAttemptResponse?> Handle(SubmitAttemptCommand request, CancellationToken cancellationToken)
        {
            var exercise = await _db.Exercises.AsNoTracking()
                .Include(item => item.Synonyms)
                .Include(item => item.Modes)
                .FirstOrDefaultAsync(item => item.Id == request.ExerciseId, cancellationToken);
            if (exercise is null || exercise.Modes.All(mode => mode.Difficulty != request.Difficulty))
            {
                return null;
            }

            var given = AnswerText.Normalize(request.Answer);
            var accepted = exercise.Synonyms
                .Select(item => item.Text)
                .Prepend(exercise.Answer)
                .Select(AnswerText.Normalize);
            var correct = given.Length > 0 && accepted.Contains(given);
            var diff = correct
                ? []
                : AnswerDiff.Between(AnswerText.ForDisplay(exercise.Answer), AnswerText.ForDisplay(request.Answer));

            return new SubmitAttemptResponse(
                correct ? ExerciseStatus.Success.ToString() : ExerciseStatus.Failure.ToString(),
                exercise.Source,
                request.Answer,
                exercise.Answer,
                diff);
        }
    }
}
