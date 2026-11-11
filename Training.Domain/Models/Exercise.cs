using Training.Domain.Enums;

namespace Training.Domain.Models
{
    public class Exercise
    {
        public int Id { get; set; }
        public int LessonId { get; set; }
        public Lesson? Lesson { get; set; }
        public string Source { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public List<ExerciseSynonym> Synonyms { get; set; } = new();
        public List<ExerciseToken> Tokens { get; set; } = new();
        public List<ExerciseMode> Modes { get; set; } = new();
    }
}
