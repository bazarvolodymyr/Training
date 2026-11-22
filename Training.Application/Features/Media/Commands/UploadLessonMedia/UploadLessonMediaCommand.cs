using MediatR;
using Training.Domain.Enums;

namespace Training.Application.Features.Media.Commands.UploadLessonMedia
{
    public record UploadLessonMediaCommand(int LessonId, string FileName, Stream Content)
        : IRequest<UploadLessonMediaResult>;

    public enum UploadLessonMediaStatus
    {
        Uploaded,
        LessonNotFound,
        InvalidType,
        TooLarge,
        LimitReached
    }

    public record UploadLessonMediaResult(
        UploadLessonMediaStatus Status,
        int? Id = null,
        string? Url = null,
        string? ContentType = null,
        MediaKind? Kind = null);
}
