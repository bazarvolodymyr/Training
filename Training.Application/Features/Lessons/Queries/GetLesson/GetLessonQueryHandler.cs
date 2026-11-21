using MediatR;
using Microsoft.EntityFrameworkCore;
using Training.Application.Interfaces;

namespace Training.Application.Features.Lessons.Queries.GetLesson
{
    public class GetLessonQueryHandler : IRequestHandler<GetLessonQuery, LessonArticleResponse?>
    {
        private readonly ITrainingDbContext _db;
        private readonly ArticleHtmlSanitizer _sanitizer;

        public GetLessonQueryHandler(ITrainingDbContext db, ArticleHtmlSanitizer sanitizer)
        {
            _db = db;
            _sanitizer = sanitizer;
        }

        public async Task<LessonArticleResponse?> Handle(GetLessonQuery request, CancellationToken cancellationToken)
        {
            var lesson = await _db.Lessons.AsNoTracking()
                .Where(item => item.Id == request.Id && item.IsPublished)
                .Select(item => new
                {
                    item.Id,
                    item.Name,
                    item.ArticleHtml,
                    item.SourceLang,
                    item.TargetLang
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (lesson is null)
            {
                return null;
            }

            return new LessonArticleResponse(
                lesson.Id,
                lesson.Name,
                _sanitizer.Sanitize(lesson.ArticleHtml),
                lesson.SourceLang,
                lesson.TargetLang);
        }
    }
}
