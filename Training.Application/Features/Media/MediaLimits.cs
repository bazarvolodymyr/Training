namespace Training.Application.Features.Media
{
    public static class MediaLimits
    {
        public const int MaxImageBytes = 2 * 1024 * 1024;
        public const int MaxAudioBytes = 5 * 1024 * 1024;
        public const int MaxRequestBytes = 6 * 1024 * 1024;
        public const int MaxImagesPerLesson = 20;
        public const int MaxAudioPerLesson = 10;

        public static string PublicUrl(int id) => $"/api/v1/media/{id}";
    }
}
