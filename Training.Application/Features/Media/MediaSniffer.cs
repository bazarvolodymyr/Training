using Training.Domain.Enums;

namespace Training.Application.Features.Media
{
    public static class MediaSniffer
    {
        public readonly record struct Detected(MediaKind Kind, string ContentType, string Extension);

        public static Detected? Detect(ReadOnlySpan<byte> header)
        {
            if (IsJpeg(header))
            {
                return new Detected(MediaKind.Image, "image/jpeg", ".jpg");
            }

            if (IsPng(header))
            {
                return new Detected(MediaKind.Image, "image/png", ".png");
            }

            if (IsWebp(header))
            {
                return new Detected(MediaKind.Image, "image/webp", ".webp");
            }

            if (IsMp3(header))
            {
                return new Detected(MediaKind.Audio, "audio/mpeg", ".mp3");
            }

            if (IsM4a(header))
            {
                return new Detected(MediaKind.Audio, "audio/mp4", ".m4a");
            }

            return null;
        }

        private static bool IsJpeg(ReadOnlySpan<byte> header)
        {
            return header.Length >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF;
        }

        private static bool IsPng(ReadOnlySpan<byte> header)
        {
            return header.Length >= 8
                && header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47
                && header[4] == 0x0D && header[5] == 0x0A && header[6] == 0x1A && header[7] == 0x0A;
        }

        private static bool IsWebp(ReadOnlySpan<byte> header)
        {
            return header.Length >= 12
                && header[0] == (byte)'R' && header[1] == (byte)'I' && header[2] == (byte)'F' && header[3] == (byte)'F'
                && header[8] == (byte)'W' && header[9] == (byte)'E' && header[10] == (byte)'B' && header[11] == (byte)'P';
        }

        private static bool IsMp3(ReadOnlySpan<byte> header)
        {
            if (header.Length >= 3 && header[0] == (byte)'I' && header[1] == (byte)'D' && header[2] == (byte)'3')
            {
                return true;
            }

            return header.Length >= 2 && header[0] == 0xFF && (header[1] & 0xE0) == 0xE0;
        }

        private static bool IsM4a(ReadOnlySpan<byte> header)
        {
            if (header.Length < 12
                || header[4] != (byte)'f' || header[5] != (byte)'t' || header[6] != (byte)'y' || header[7] != (byte)'p')
            {
                return false;
            }

            var brand = System.Text.Encoding.ASCII.GetString(header.Slice(8, 4));
            return brand is "M4A " or "M4B " or "mp41" or "mp42";
        }
    }
}
