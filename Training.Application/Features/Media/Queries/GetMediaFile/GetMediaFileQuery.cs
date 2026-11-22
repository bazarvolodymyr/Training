using MediatR;

namespace Training.Application.Features.Media.Queries.GetMediaFile
{
    public record GetMediaFileQuery(int Id, bool IncludeUnpublished) : IRequest<MediaFileResponse?>;

    public record MediaFileResponse(string ContentType, string StoredFileName);
}
