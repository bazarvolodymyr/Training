using Ganss.Xss;

namespace Training.Application.Features.Lessons
{
    public class ArticleHtmlSanitizer
    {
        private static readonly string[] Tags =
        [
            "a", "audio", "b", "blockquote", "br", "div", "em", "figcaption", "figure",
            "h1", "h2", "h3", "h4", "hr", "i", "img", "li", "ol", "p", "s", "source",
            "strong", "table", "tbody", "td", "th", "thead", "tr", "u", "ul"
        ];

        private static readonly string[] Attributes =
        [
            "alt", "colspan", "controls", "href", "rowspan", "src"
        ];

        public string Sanitize(string? html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return string.Empty;
            }

            var sanitizer = new HtmlSanitizer();
            sanitizer.AllowedTags.Clear();
            sanitizer.AllowedAttributes.Clear();
            sanitizer.AllowedSchemes.Clear();
            sanitizer.AllowedCssProperties.Clear();
            foreach (var tag in Tags)
            {
                sanitizer.AllowedTags.Add(tag);
            }

            foreach (var attribute in Attributes)
            {
                sanitizer.AllowedAttributes.Add(attribute);
            }

            sanitizer.AllowedSchemes.Add("http");
            sanitizer.AllowedSchemes.Add("https");
            sanitizer.AllowDataAttributes = false;
            sanitizer.FilterUrl += (_, args) =>
            {
                var tag = args.Tag.LocalName;
                if (!tag.Equals("img", StringComparison.OrdinalIgnoreCase)
                    && !tag.Equals("audio", StringComparison.OrdinalIgnoreCase)
                    && !tag.Equals("source", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                var url = args.SanitizedUrl;
                if (string.IsNullOrEmpty(url) || !IsStoredMedia(url))
                {
                    args.SanitizedUrl = null;
                }
            };
            sanitizer.PostProcessNode += (_, args) =>
            {
                if (args.Node is not AngleSharp.Dom.IElement element)
                {
                    return;
                }

                if (element.LocalName is "img" or "audio" or "source" && !element.HasAttribute("src"))
                {
                    element.Remove();
                }
            };

            return sanitizer.Sanitize(html);
        }

        private static bool IsStoredMedia(string url)
        {
            const string prefix = "/api/v1/media/";
            if (!url.StartsWith(prefix, StringComparison.Ordinal))
            {
                return false;
            }

            var id = url[prefix.Length..];
            return int.TryParse(id, out var parsed) && parsed > 0 && id == parsed.ToString();
        }
    }
}
