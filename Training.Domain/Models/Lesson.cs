namespace Training.Domain.Models
{
    public class Lesson
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ArticleHtml { get; set; } = string.Empty;
        public bool IsPublished { get; set; }
        public int SortOrder { get; set; }
        public string SourceLang { get; set; } = string.Empty;
        public string TargetLang { get; set; } = string.Empty;
        public List<Exercise> Exercises { get; set; } = new();
    }
}
