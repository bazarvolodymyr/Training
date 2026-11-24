namespace Training.Application.Features.Quiz
{
    public static class AnswerDiff
    {
        public static IReadOnlyList<DiffSegment> Between(string expected, string actual)
        {
            var scores = new int[expected.Length + 1, actual.Length + 1];
            for (var i = expected.Length - 1; i >= 0; i--)
            {
                for (var j = actual.Length - 1; j >= 0; j--)
                {
                    scores[i, j] = Same(expected[i], actual[j])
                        ? scores[i + 1, j + 1] + 1
                        : Math.Max(scores[i + 1, j], scores[i, j + 1]);
                }
            }

            var parts = new List<(string Op, char Text)>();
            var left = 0;
            var right = 0;
            while (left < expected.Length && right < actual.Length)
            {
                if (Same(expected[left], actual[right]))
                {
                    parts.Add(("equal", expected[left]));
                    left++;
                    right++;
                }
                else if (scores[left + 1, right] >= scores[left, right + 1])
                {
                    parts.Add(("insert", expected[left]));
                    left++;
                }
                else
                {
                    parts.Add(("delete", actual[right]));
                    right++;
                }
            }

            while (left < expected.Length)
            {
                parts.Add(("insert", expected[left]));
                left++;
            }

            while (right < actual.Length)
            {
                parts.Add(("delete", actual[right]));
                right++;
            }

            return Merge(parts);
        }

        private static bool Same(char expected, char actual)
        {
            return char.ToLowerInvariant(expected) == char.ToLowerInvariant(actual);
        }

        private static IReadOnlyList<DiffSegment> Merge(List<(string Op, char Text)> parts)
        {
            var segments = new List<DiffSegment>();
            foreach (var part in parts)
            {
                if (segments.Count > 0 && segments[^1].Op == part.Op)
                {
                    segments[^1] = segments[^1] with { Text = segments[^1].Text + part.Text };
                    continue;
                }

                segments.Add(new DiffSegment(part.Op, part.Text.ToString()));
            }

            return segments;
        }
    }

    public record DiffSegment(string Op, string Text);
}
