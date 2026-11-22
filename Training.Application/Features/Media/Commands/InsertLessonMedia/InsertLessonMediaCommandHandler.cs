using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Training.Application.Features.Lessons;
using Training.Application.Interfaces;
using Training.Application.Features.Media;
using Training.Domain.Enums;

namespace Training.Application.Features.Media.Commands.InsertLessonMedia
{
    public class InsertLessonMediaCommandHandler : IRequestHandler<InsertLessonMediaCommand, InsertLessonMediaStatus>
    {
        private readonly ITrainingDbContext _db;
        private readonly ArticleHtmlSanitizer _sanitizer;

        public InsertLessonMediaCommandHandler(ITrainingDbContext db, ArticleHtmlSanitizer sanitizer)
        {
            _db = db;
            _sanitizer = sanitizer;
        }

        public async Task<InsertLessonMediaStatus> Handle(InsertLessonMediaCommand request, CancellationToken cancellationToken)
        {
            var lesson = await _db.Lessons.FirstOrDefaultAsync(item => item.Id == request.LessonId, cancellationToken);
            if (lesson is null)
            {
                return InsertLessonMediaStatus.LessonNotFound;
            }

            var asset = await _db.MediaAssets.AsNoTracking()
                .FirstOrDefaultAsync(
                    item => item.Id == request.MediaId && item.LessonId == request.LessonId,
                    cancellationToken);
            if (asset is null)
            {
                return InsertLessonMediaStatus.MediaNotFound;
            }

            var url = MediaLimits.PublicUrl(asset.Id);
            var tag = asset.Kind == MediaKind.Image
                ? $"""<img src="{url}" alt="{WebUtility.HtmlEncode(request.Alt ?? string.Empty)}">"""
                : $"""<audio src="{url}" controls></audio>""";
            lesson.ArticleHtml = _sanitizer.Sanitize(lesson.ArticleHtml + tag);
            await _db.SaveChangesAsync(cancellationToken);
            return InsertLessonMediaStatus.Inserted;
        }
    }
}
