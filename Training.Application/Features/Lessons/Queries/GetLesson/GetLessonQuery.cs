using MediatR;

namespace Training.Application.Features.Lessons.Queries.GetLesson
{
    public record GetLessonQuery(int Id) : IRequest<LessonArticleResponse?>;

    public record LessonArticleResponse(
        int Id,
        string Name,
        string ArticleHtml,
        string SourceLang,
        string TargetLang);
}
