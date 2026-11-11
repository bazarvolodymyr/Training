namespace Training.Domain.Models
{
    public class ExerciseDistractor
    {
        public int Id { get; set; }
        public int TokenId { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
