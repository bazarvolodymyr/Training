namespace Training.Domain.Models
{
    public class ExerciseSynonym
    {
        public int Id { get; set; }
        public int ExerciseId { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
