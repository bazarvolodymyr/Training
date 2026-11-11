namespace Training.Domain.Models
{
    public class ExerciseToken
    {
        public int Id { get; set; }
        public int ExerciseId { get; set; }
        public string Text { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public List<ExerciseDistractor> Distractors { get; set; } = new();
    }
}
