using Training.Domain.Enums;

namespace Training.Domain.Models
{
    public class MediaAsset
    {
        public int Id { get; set; }
        public int LessonId { get; set; }
        public Lesson? Lesson { get; set; }
        public MediaKind Kind { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public string OriginalFileName { get; set; } = string.Empty;
        public string StoredFileName { get; set; } = string.Empty;
        public long SizeBytes { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
