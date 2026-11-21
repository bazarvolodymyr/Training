using MediatR;
using Microsoft.EntityFrameworkCore;
using Training.Application.Interfaces;

namespace Training.Application.Features.Lessons.Queries.ListLessons
{
    public class ListLessonsQueryHandler : IRequestHandler<ListLessonsQuery, IReadOnlyList<LessonListItemResponse>>
    {
        private readonly ITrainingDbContext _db;

        public ListLessonsQueryHandler(ITrainingDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<LessonListItemResponse>> Handle(ListLessonsQuery request, CancellationToken cancellationToken)
        {
            return await _db.Lessons.AsNoTracking()
                .Where(lesson => lesson.IsPublished)
                .OrderBy(lesson => lesson.SortOrder)
                .ThenBy(lesson => lesson.Id)
                .Select(lesson => new LessonListItemResponse(lesson.Id, lesson.Name, lesson.SourceLang, lesson.TargetLang))
                .ToListAsync(cancellationToken);
        }
    }
}
