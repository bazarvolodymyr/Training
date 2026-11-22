using MediatR;
using Microsoft.EntityFrameworkCore;
using Training.Application.Interfaces;
using Training.Domain.Enums;
using Training.Domain.Models;

namespace Training.Application.Features.Media.Commands.UploadLessonMedia
{
    public class UploadLessonMediaCommandHandler : IRequestHandler<UploadLessonMediaCommand, UploadLessonMediaResult>
    {
        private readonly ITrainingDbContext _db;
        private readonly IMediaFileStore _files;

        public UploadLessonMediaCommandHandler(ITrainingDbContext db, IMediaFileStore files)
        {
            _db = db;
            _files = files;
        }

        public async Task<UploadLessonMediaResult> Handle(UploadLessonMediaCommand request, CancellationToken cancellationToken)
        {
            var lessonExists = await _db.Lessons.AnyAsync(lesson => lesson.Id == request.LessonId, cancellationToken);
            if (!lessonExists)
            {
                return new UploadLessonMediaResult(UploadLessonMediaStatus.LessonNotFound);
            }

            var header = new byte[16];
            var headerRead = await request.Content.ReadAsync(header, cancellationToken);
            var detected = MediaSniffer.Detect(header.AsSpan(0, headerRead));
            if (detected is null)
            {
                return new UploadLessonMediaResult(UploadLessonMediaStatus.InvalidType);
            }

            var maxBytes = detected.Value.Kind == MediaKind.Image
                ? MediaLimits.MaxImageBytes
                : MediaLimits.MaxAudioBytes;
            var payload = new MemoryStream();
            payload.Write(header, 0, headerRead);
            var chunk = new byte[8192];
            while (true)
            {
                var read = await request.Content.ReadAsync(chunk, cancellationToken);
                if (read == 0)
                {
                    break;
                }

                if (payload.Length + read > maxBytes)
                {
                    return new UploadLessonMediaResult(UploadLessonMediaStatus.TooLarge);
                }

                payload.Write(chunk, 0, read);
            }

            if (payload.Length == 0)
            {
                return new UploadLessonMediaResult(UploadLessonMediaStatus.InvalidType);
            }

            var count = await _db.MediaAssets.CountAsync(
                asset => asset.LessonId == request.LessonId && asset.Kind == detected.Value.Kind,
                cancellationToken);
            var limit = detected.Value.Kind == MediaKind.Image
                ? MediaLimits.MaxImagesPerLesson
                : MediaLimits.MaxAudioPerLesson;
            if (count >= limit)
            {
                return new UploadLessonMediaResult(UploadLessonMediaStatus.LimitReached);
            }

            var storedName = $"{Guid.NewGuid():N}{detected.Value.Extension}";
            payload.Position = 0;
            await _files.SaveAsync(storedName, payload, cancellationToken);

            var asset = new MediaAsset
            {
                LessonId = request.LessonId,
                Kind = detected.Value.Kind,
                ContentType = detected.Value.ContentType,
                OriginalFileName = CleanFileName(request.FileName),
                StoredFileName = storedName,
                SizeBytes = payload.Length,
                CreatedAt = DateTimeOffset.UtcNow
            };
            _db.MediaAssets.Add(asset);
            try
            {
                await _db.SaveChangesAsync(cancellationToken);
            }
            catch
            {
                _files.Delete(storedName);
                throw;
            }

            return new UploadLessonMediaResult(
                UploadLessonMediaStatus.Uploaded,
                asset.Id,
                MediaLimits.PublicUrl(asset.Id),
                asset.ContentType,
                asset.Kind);
        }

        private static string CleanFileName(string fileName)
        {
            var name = Path.GetFileName(fileName);
            if (string.IsNullOrWhiteSpace(name))
            {
                return "file";
            }

            return name.Length <= 200 ? name : name[..200];
        }
    }
}
