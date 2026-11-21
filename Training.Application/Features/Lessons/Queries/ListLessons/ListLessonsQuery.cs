using MediatR;

namespace Training.Application.Features.Lessons.Queries.ListLessons
{
    public record ListLessonsQuery : IRequest<IReadOnlyList<LessonListItemResponse>>;

    public record LessonListItemResponse(int Id, string Name, string SourceLang, string TargetLang);
}
