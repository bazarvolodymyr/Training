using MediatR;
using Microsoft.EntityFrameworkCore;
using Training.Application.Interfaces;

namespace Training.Application.Features.Media.Queries.GetMediaFile
{
    public class GetMediaFileQueryHandler : IRequestHandler<GetMediaFileQuery, MediaFileResponse?>
    {
        private readonly ITrainingDbContext _db;

        public GetMediaFileQueryHandler(ITrainingDbContext db)
        {
            _db = db;
        }

        public async Task<MediaFileResponse?> Handle(GetMediaFileQuery request, CancellationToken cancellationToken)
        {
            var asset = await _db.MediaAssets.AsNoTracking()
                .Where(item => item.Id == request.Id)
                .Where(item => request.IncludeUnpublished || item.Lesson!.IsPublished)
                .Select(item => new { item.ContentType, item.StoredFileName })
                .FirstOrDefaultAsync(cancellationToken);

            return asset is null ? null : new MediaFileResponse(asset.ContentType, asset.StoredFileName);
        }
    }
}
