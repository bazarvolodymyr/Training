using MediatR;
using Microsoft.EntityFrameworkCore;
using Training.Application.Interfaces;

namespace Training.Application.Features.Quiz.Queries.GetQuestion
{
    public class GetQuestionQueryHandler : IRequestHandler<GetQuestionQuery, GetQuestionResponse?>
    {
        private readonly ITrainingDbContext _db;

        public GetQuestionQueryHandler(ITrainingDbContext db)
        {
            _db = db;
        }

        public async Task<GetQuestionResponse?> Handle(GetQuestionQuery request, CancellationToken cancellationToken)
        {
            return await _db.Exercises.AsNoTracking()
                .Where(exercise => exercise.Lesson!.IsPublished)
                .OrderBy(exercise => exercise.Lesson!.SortOrder)
                .ThenBy(exercise => exercise.SortOrder)
                .ThenBy(exercise => exercise.Id)
                .Select(exercise => new GetQuestionResponse
                {
                    Id = exercise.Id,
                    Question = exercise.Source
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
