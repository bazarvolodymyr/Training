using MediatR;

namespace Training.Application.Features.Media.Commands.InsertLessonMedia
{
    public record InsertLessonMediaCommand(int LessonId, int MediaId, string? Alt) : IRequest<InsertLessonMediaStatus>;

    public enum InsertLessonMediaStatus
    {
        Inserted,
        LessonNotFound,
        MediaNotFound
    }
}
