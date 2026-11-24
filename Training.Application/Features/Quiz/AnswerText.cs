using System.Text;

namespace Training.Application.Features.Quiz
{
    public static class AnswerText
    {
        public static string Normalize(string? value)
        {
            return Clean(value, lowerCase: true);
        }

        public static string ForDisplay(string? value)
        {
            return Clean(value, lowerCase: false);
        }

        private static string Clean(string? value, bool lowerCase)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            var builder = new StringBuilder(value.Length);
            var pendingSpace = false;
            foreach (var ch in value.Trim())
            {
                if (char.IsWhiteSpace(ch))
                {
                    pendingSpace = builder.Length > 0;
                    continue;
                }

                if (IsApostrophe(ch))
                {
                    if (pendingSpace)
                    {
                        builder.Append(' ');
                        pendingSpace = false;
                    }

                    builder.Append(ch);
                    continue;
                }

                if (char.IsPunctuation(ch) || char.IsSymbol(ch))
                {
                    continue;
                }

                if (pendingSpace)
                {
                    builder.Append(' ');
                    pendingSpace = false;
                }

                builder.Append(lowerCase ? char.ToLowerInvariant(ch) : ch);
            }

            return builder.ToString();
        }

        private static bool IsApostrophe(char ch)
        {
            return ch is '\'' or '\u2019' or '\u02BC';
        }
    }
}
