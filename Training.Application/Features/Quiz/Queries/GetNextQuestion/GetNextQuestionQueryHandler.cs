using MediatR;
using Microsoft.EntityFrameworkCore;
using Training.Application.Interfaces;
using Training.Domain.Enums;

namespace Training.Application.Features.Quiz.Queries.GetNextQuestion
{
    public class GetNextQuestionQueryHandler : IRequestHandler<GetNextQuestionQuery, NextQuestionResponse?>
    {
        private readonly ITrainingDbContext _db;

        public GetNextQuestionQueryHandler(ITrainingDbContext db)
        {
            _db = db;
        }

        public async Task<NextQuestionResponse?> Handle(GetNextQuestionQuery request, CancellationToken cancellationToken)
        {
            var query = _db.Exercises.AsNoTracking()
                .Where(exercise => exercise.LessonId == request.LessonId && exercise.Lesson!.IsPublished)
                .Where(exercise => exercise.Modes.Any(mode => mode.Difficulty == request.Difficulty));
            if (request.Difficulty != Difficulty.Hard)
            {
                query = query.Include(exercise => exercise.Tokens).ThenInclude(token => token.Distractors);
            }

            var exercise = await query
                .OrderBy(exercise => exercise.SortOrder)
                .ThenBy(exercise => exercise.Id)
                .FirstOrDefaultAsync(cancellationToken);
            if (exercise is null)
            {
                return null;
            }

            return new NextQuestionResponse(
                exercise.Id,
                exercise.LessonId,
                exercise.Source,
                request.Difficulty.ToString(),
                request.Difficulty == Difficulty.Hard ? [] : Bank(exercise.Tokens, request.Difficulty));
        }

        private static IReadOnlyList<string> Bank(
            IEnumerable<Training.Domain.Models.ExerciseToken> tokens,
            Difficulty difficulty)
        {
            var bank = new List<string>();
            foreach (var token in tokens.OrderBy(item => item.SortOrder))
            {
                bank.Add(token.Text);
                if (difficulty == Difficulty.Medium)
                {
                    bank.AddRange(token.Distractors.Select(item => item.Text));
                }
            }

            for (var i = bank.Count - 1; i > 0; i--)
            {
                var swap = Random.Shared.Next(i + 1);
                (bank[i], bank[swap]) = (bank[swap], bank[i]);
            }

            return bank;
        }
    }
}
